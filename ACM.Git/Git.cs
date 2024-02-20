using System.Globalization;
using System.Text;
using System.Threading;
using ACM.Kernel;
using APC.Kernel;
using CliWrap;
using Foundatio.Storage;
using Minio.Exceptions;
using Polly;
using Polly.Registry;

namespace ACM.Git;

public class Git {
  private const string INCREMENT_FORMAT_ = "yyyy-MM-ddTHH:mm:ssZ";
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
    logger_.LogDebug($"{remote}: Starting");
    bool success =
      await git_timeout_.ExecuteAsync(async (state, token) =>
                                        await CloneOrUpdateLocalMirror(
                                          state, token), repository);
    logger_.LogDebug($"{remote}: {success}");
    if (success) {
      logger_.LogDebug($"{remote}: Creating bundle");
      await CreateIncrementalGitBundle(repository, token);
    }

    return true;
  }

  private async Task<bool> CloneOrUpdateLocalMirror(
    Repository repository, CancellationToken token = default) {
    if (!Directory.Exists(repository.LocalPath)) {
      Directory.CreateDirectory(Path.Join(dir_, repository.Owner));
      // Clone the mirror repository
      logger_.LogInformation($"{repository.Remote}: Cloning initial repository.");

      return await Bin.Execute("git",
                               args => {
                                 args.Add("clone");
                                 args.Add("--mirror");
                                 args.Add(repository.Remote);
                                 args.Add(repository.LocalPath);
                               }, logger_, token: token);
    }

    // Fetch updates to the mirror repository
    logger_.LogDebug($"{repository.Remote}: Fetching updates.");
    return await Bin.Execute("git", args => {
      args.Add("remote");
      args.Add("update");
      args.Add("--prune");
    }, logger_, repository.LocalPath, token: token);
  }

  private async Task CreateIncrementalGitBundle(Repository repository, CancellationToken token) {
    string bundle_dir = Path.Join(bundle_dir_, repository.Owner);
    if (!Directory.Exists(bundle_dir)) {
      Directory.CreateDirectory(bundle_dir);
    }

    DateTime now = DateTime.UtcNow;
    /* Get latest update from storage */
    logger_.LogDebug($"{repository.Remote}: Getting timestamp");
    DateTime reference_date = await GetLastTimestamp(repository);

    // Calculate the range of commits based on the reference date
    string since_date = reference_date.ToString(INCREMENT_FORMAT_);
    string until_date = now.ToString(INCREMENT_FORMAT_);

    string bundle_file_name =
      $"{repository.Name}@{reference_date:yyyyMMddHHmmss}-{now:yyyyMMddHHmmss}.bundle";
    string bundle_file_path = Path.Combine(bundle_dir, bundle_file_name);

    // Create an incremental bundle
    logger_.LogInformation($"{repository.Remote}: Bundling {since_date} ({since_date}) - {until_date}");
    logger_.LogDebug($"{repository.Remote}: Dirs {repository.LocalPath} - {repository.Directory}");

    bool success = await Bin.Execute("git", args => {
      args.Add("bundle");
      args.Add("create");
      args.Add(bundle_file_path);
      args.Add($"--since=\"{since_date}\"");
      //args.Add($"--until=\"{until_date}\"");
      args.Add($"--all");
    }, logger_, repository.LocalPath, 0, token);
    logger_.LogDebug($"{repository.Remote}: Bundle result {success}");
    if (success) {
      logger_.LogInformation($"{repository.Remote}: Pushing {bundle_file_path} to S3.");
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
    string path = Path.Join("git", repository.Owner, $"{repository.Name}@*-*");
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
    logger_.LogDebug($"Opening: {bundle_file_path}");
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