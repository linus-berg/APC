using System.Globalization;
using ACM.Kernel;
using APC.Kernel;
using Foundatio.Storage;
using Minio.Exceptions;
using Polly;
using Polly.Registry;

namespace ACM.Git;

public class Git {
  private const string C_INCREMENT_FORMAT_ = "yyyy-MM-ddTHH:mm:ssZ";
  private readonly string bundle_dir_;
  private readonly string dir_;
  private readonly FileSystem fs_;
  private readonly ResiliencePipeline<bool> git_timeout_;
  private readonly ILogger<Git> logger_;

  public Git(FileSystem fs, ResiliencePipelineProvider<string> polly,
             ILogger<Git> logger) {
    fs_ = fs;
    dir_ = fs_.GetModuleDir("git", true);
    bundle_dir_ = Path.GetFullPath(Path.Join(dir_, "/tmp", "/bundles"));
    git_timeout_ = polly.GetPipeline<bool>("git-timeout");
    logger_ = logger;
    ConfigureProxy();
  }

  private void ConfigureProxy() {
    Bin
      .Execute(
        "git",
        args => {
          args.Add("config");
          args.Add("--global");
          args.Add("http.proxy");
          args.Add(Environment.GetEnvironmentVariable("HTTPS_PROXY"));
        }, logger_).Wait();
  }

  public async Task<bool> Mirror(string remote, CancellationToken token) {
    Repository repository = new(remote, dir_);
    logger_.LogDebug("{Remote}: Starting", remote);
    bool success =
      await git_timeout_.ExecuteAsync(async (state, lambda_token) =>
                                        await CloneOrUpdateLocalMirror(
                                          state, lambda_token), repository,
                                      token);
    logger_.LogDebug("{Remote}: {Success}", remote, success);
    if (success) {
      logger_.LogDebug("{Remote}: Creating bundle", remote);
      await CreateIncrementalGitBundle(repository, token);
    }

    return true;
  }

  private async Task<bool> CloneOrUpdateLocalMirror(
    Repository repository, CancellationToken token = default) {
    if (!Directory.Exists(repository.local_path)) {
      Directory.CreateDirectory(Path.Join(dir_, repository.owner));
      // Clone the mirror repository
      logger_.LogInformation("{RepositoryRemote}: Cloning initial repository",
                             repository.remote);

      return await Bin.Execute("git",
                               args => {
                                 args.Add("clone");
                                 args.Add("--mirror");
                                 args.Add(repository.remote);
                                 args.Add(repository.local_path);
                               }, logger_, token: token);
    }

    // Fetch updates to the mirror repository
    logger_.LogDebug("{RepositoryRemote}: Fetching updates", repository.remote);
    return await Bin.Execute("git", args => {
      args.Add("remote");
      args.Add("update");
      args.Add("--prune");
    }, logger_, repository.local_path, token: token);
  }

  private async Task CreateIncrementalGitBundle(Repository repository,
                                                CancellationToken token) {
    string bundle_dir = Path.Join(bundle_dir_, repository.owner);
    if (!Directory.Exists(bundle_dir)) {
      Directory.CreateDirectory(bundle_dir);
    }

    DateTime now = DateTime.UtcNow;
    /* Get latest update from storage */
    logger_.LogDebug("{RepositoryRemote}: Getting timestamp",
                     repository.remote);
    DateTime reference_date = await GetLastTimestamp(repository);

    // Calculate the range of commits based on the reference date
    string since_date = reference_date.ToString(C_INCREMENT_FORMAT_);
    string until_date = now.ToString(C_INCREMENT_FORMAT_);

    string bundle_file_name =
      $"{repository.name}@{reference_date:yyyyMMddHHmmss}-{now:yyyyMMddHHmmss}.bundle";
    string bundle_file_path = Path.Combine(bundle_dir, bundle_file_name);

    // Create an incremental bundle
    logger_.LogInformation(
      "{RepositoryRemote}: Bundling {SinceDate} - {UntilDate}",
      repository.remote, since_date, until_date);
    logger_.LogDebug(
      "{RepositoryRemote}: Dirs {RepositoryLocalPath} - {RepositoryDirectory}",
      repository.remote, repository.local_path, repository.directory);

    bool success = await Bin.Execute("git", args => {
      args.Add("bundle");
      args.Add("create");
      args.Add(bundle_file_path);
      args.Add($"--since=\"{since_date}\"");
      //args.Add($"--until=\"{until_date}\"");
      args.Add("--all");
    }, logger_, repository.local_path, 0, token);
    logger_.LogDebug("{RepositoryRemote}: Bundle result {Success}",
                     repository.remote, success);
    if (success) {
      logger_.LogInformation(
        "{RepositoryRemote}: Pushing {BundleFilePath} to S3", repository.remote,
        bundle_file_path);
      bool uploaded = await PushToStorage(bundle_file_path);
      if (uploaded) {
        await fs_.CreateDeltaLink(
          "git",
          $"git://{Path.GetRelativePath(bundle_dir_, bundle_file_path)}");
      } else {
        logger_.LogError("Failed to push {BundleFilePath} to storage",
                         bundle_file_path);
      }
    }
  }

  private async Task<DateTime> GetLastTimestamp(Repository repository) {
    string path = Path.Join("git", repository.owner, $"{repository.name}@*-*");
    IReadOnlyCollection<FileSpec> files = await fs_.GetFileList(path);
    if (files.Count == 0) {
      return DateTime.UnixEpoch;
    }

    string timestamp = Path.GetFileNameWithoutExtension(files.Last().Path)
                           .Split("@").Last().Split("-").Last();
    return DateTime.TryParseExact(timestamp, "yyyyMMddHHmmss", null,
                                  DateTimeStyles.None,
                                  out DateTime reference_date)
             ? reference_date
             : DateTime.UnixEpoch;
  }


  private async Task<bool> PushToStorage(string bundle_file_path) {
    if (!File.Exists(bundle_file_path)) {
      throw new FileNotFoundException(
        $"{bundle_file_path} not found on disk.");
    }

    /* Open bundle and stream to S3 */
    logger_.LogDebug("Opening: {BundleFilePath}", bundle_file_path);
    await using Stream stream = File.OpenRead(bundle_file_path);
    string storage_path =
      Path.Join("git", Path.GetRelativePath(bundle_dir_, bundle_file_path));

    /* Try uploading to S3. */
    bool success = await fs_.PutFile(storage_path, stream);
    stream.Close();

    /* Delete bundle file on disk */
    File.Delete(bundle_file_path);

    /* If S3 upload failed */
    if (!success) {
      throw new MinioException($"Failed to upload {bundle_file_path}");
    }

    return success;
  }
}