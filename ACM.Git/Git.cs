using System.Globalization;
using ACM.Kernel;
using APC.Kernel;
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
        $"config --global http.proxy {Environment.GetEnvironmentVariable("HTTPS_PROXY")}")
      .Wait();
  }

  public async Task<bool> Mirror(string remote) {
    Repository repository = new(remote, dir_);
    bool success =
      await git_timeout_.ExecuteAsync(async (state, token) =>
                                        await CloneOrUpdateLocalMirror(
                                          state, token), repository);
    if (success) {
      await CreateIncrementalGitBundle(repository);
    }

    return true;
  }

  private async Task<bool> CloneOrUpdateLocalMirror(
    Repository repository, CancellationToken token = default) {
    if (!Directory.Exists(repository.LocalPath)) {
      Directory.CreateDirectory(Path.Join(dir_, repository.Owner));
      // Clone the mirror repository
      return await Bin.Execute("git",
                               $"clone --mirror {repository.Remote} {repository.LocalPath}",
                               token: token);
    }

    // Fetch updates to the mirror repository
    return await Bin.Execute("git", "fetch --prune", repository.LocalPath,
                             token: token);
  }

  private async Task CreateIncrementalGitBundle(Repository repository) {
    string bundle_dir = Path.Join(bundle_dir_, repository.Owner);
    if (!Directory.Exists(bundle_dir)) {
      Directory.CreateDirectory(bundle_dir);
    }

    DateTime now = DateTime.UtcNow;
    /* Get latest update from storage */
    DateTime reference_date = await GetLastTimestamp(repository);

    // Calculate the range of commits based on the reference date
    string since_date = reference_date.ToString(INCREMENT_FORMAT_);
    string until_date = now.ToString(INCREMENT_FORMAT_);

    string bundle_file_name = $"{repository.Name}-{now:yyyyMMddHHmmss}.bundle";
    string bundle_file_path = Path.Combine(bundle_dir, bundle_file_name);

    // Create an incremental bundle
    bool success = await Bin.Execute("git",
                                     $"bundle create {bundle_file_path} --since=\"{since_date}\" --until=\"{until_date}\" --all",
                                     repository.LocalPath);
    if (success) {
      try {
        bool uploaded = await PushToStorage(bundle_file_path);
        if (uploaded) {
          await fs_.CreateDeltaLink(
            "git",
            $"git://{Path.GetRelativePath(bundle_dir_, bundle_file_path)}");
        } else {
          logger_.LogError("Failed to push {BundleFilePath} to storage",
                           bundle_file_path);
        }
      } catch (Exception e) {
        logger_.LogError("Failed to upload {Bundle}, {Error}", bundle_file_path,
                         e.ToString());
      }
    }
  }

  private async Task<DateTime> GetLastTimestamp(Repository repository) {
    string path = Path.Join("git", repository.Owner, $"{repository.Name}-*");
    IReadOnlyCollection<FileSpec> files = await fs_.GetFileList(path);
    if (files.Count == 0) {
      return DateTime.MinValue;
    }

    string timestamp = Path.GetFileNameWithoutExtension(files.Last().Path)
                           .Split("-").Last();
    return DateTime.TryParseExact(timestamp, "yyyyMMddHHmmss", null,
                                  DateTimeStyles.None,
                                  out DateTime reference_date)
             ? reference_date
             : DateTime.MinValue;
  }


  private async Task<bool> PushToStorage(string bundle_file_path) {
    if (!File.Exists(bundle_file_path)) {
      throw new FileNotFoundException(
        $"{bundle_file_path} not found on disk.");
    }

    /* Open bundle and stream to S3 */
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