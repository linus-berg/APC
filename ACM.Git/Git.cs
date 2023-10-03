using System.Diagnostics;
using System.Globalization;
using ACM.Kernel;
using APC.Kernel.Extensions;
using CliWrap;
using Minio.Exceptions;
using Polly;
using Polly.Registry;

namespace ACM.Git;

public class Git {
  private const string INCREMENT_FORMAT_ = "yyyy-MM-ddTHH:mm:ssZ";
  private readonly string bundle_dir_;
  private readonly string dir_;
  private readonly FileSystem fs_;
  private readonly ResiliencePipeline<bool> pipeline_;
  private readonly ILogger<Git> logger_;

  public Git(FileSystem fs, ResiliencePipelineProvider<string> polly_, ILogger<Git> logger) {
    fs_ = fs;
    dir_ = fs_.GetModuleDir("git", true);
    bundle_dir_ = Path.GetFullPath(Path.Join(dir_, "/tmp", "/bundles"));
    pipeline_ = polly_.GetPipeline<bool>("minio-retry");
    logger_ = logger;
    ConfigureProxy();
  }

  private void ConfigureProxy() {
    ExecuteGitCommand($"config --global http.proxy {Environment.GetEnvironmentVariable("HTTPS_PROXY")}");
  }

  public async Task<bool> Mirror(string remote) {
    Repository repository = new(remote, dir_);
    if (CloneOrUpdateLocalMirror(repository)) {
      await CreateIncrementalGitBundle(repository);
    }
    return true;
  }

  private bool CloneOrUpdateLocalMirror(Repository repository) {
    if (!Directory.Exists(repository.LocalPath)) {
      Directory.CreateDirectory(Path.Join(dir_, repository.Owner));
      // Clone the mirror repository
      return ExecuteGitCommand(
        $"clone --mirror {repository.Remote} {repository.LocalPath}");
    } else {
      // Fetch updates to the mirror repository
      return ExecuteGitCommand("fetch --prune", repository.LocalPath);
    }
  }

  private async Task CreateIncrementalGitBundle(Repository repository) {
    string bundle_dir = Path.Join(bundle_dir_, repository.Owner);
    if (!Directory.Exists(bundle_dir)) {
      Directory.CreateDirectory(bundle_dir);
    }

    DateTime now = DateTime.UtcNow;
    /* Get latest update from storage */
    DateTime reference_date = await GetTimestamp(repository);
    int index = await GetIndex(repository);

    // Calculate the range of commits based on the reference date
    string since_date = reference_date.ToString(INCREMENT_FORMAT_);
    string until_date = now.ToString(INCREMENT_FORMAT_);

    string bundle_file_name = $"{repository.Name}-{++index}.bundle";
    string bundle_file_path = Path.Combine(bundle_dir, bundle_file_name);

    // Create an incremental bundle
    bool success = ExecuteGitCommand(
      $"bundle create {bundle_file_path} --since=\"{since_date}\" --until=\"{until_date}\" --all",
      repository.LocalPath);
    if (success) {
      bool uploaded =
        await pipeline_.ExecuteAsync(async token =>
                                       await PushToStorage(bundle_file_path));
      if (uploaded) {
        await pipeline_.ExecuteAsync(async token => {
          await UpdateIndex(repository, index);
          await UpdateTimestamp(repository, now);
          return true;
        });
        await fs_.CreateDeltaLink(
          "git",
          $"git://{Path.GetRelativePath(bundle_dir_, bundle_file_path)}");
      } else {
        logger_.LogError($"Failed to push {bundle_file_path} to storage");
        if (File.Exists(bundle_file_path)) {
          File.Delete(bundle_file_path);
        }
      }
    }
  }

  private async Task<int> GetIndex(Repository repository) {
    string path = GetIndexPath(repository);
    bool exists = await fs_.Exists(path);

    if (exists) {
      string index = await fs_.GetString(path);
      return int.Parse(index);
    }

    return -1;
  }

  private async Task<DateTime> GetTimestamp(Repository repository) {
    string path = GetTimestampPath(repository);
    bool exists = await fs_.Exists(path);

    if (exists) {
      string timestamp = await fs_.GetString(path);
      if (DateTime.TryParseExact(timestamp, "yyyyMMddHHmmss", null,
                                 DateTimeStyles.None,
                                 out DateTime reference_date)) {
        return reference_date;
      }
    }

    return DateTime.MinValue;
  }

  private async Task<bool> PushToStorage(string bundle_file_path) {
    await using Stream stream = File.OpenRead(bundle_file_path);
    string storage_path =
      Path.Join("git", Path.GetRelativePath(bundle_dir_, bundle_file_path));
    bool success = await fs_.PutFile(storage_path, stream);
    stream.Close();
    if (success) {
      File.Delete(bundle_file_path);
    } else {
      throw new MinioException($"Failed to upload {bundle_file_path}");
    }

    return success;
  }

  private async Task<bool> UpdateIndex(Repository repository, int index) {
    bool success = await fs_.PutString(GetIndexPath(repository),
                               (index + 1).ToString());
    if (!success) {
      string error = $"{repository.Owner} - Failed to put index file";
      logger_.LogError(error);
      throw new MinioException(error);
    }

    return success;
  }

  private async Task<bool> UpdateTimestamp(Repository repository,
                                           DateTime timestamp) {
    bool success = await fs_.PutString(GetTimestampPath(repository),
                               $"{timestamp:yyyyMMddHHmmss}");
    if (!success) {
      string error = $"{repository.Owner} - Failed to put timestamp file";
      logger_.LogError(error);
      throw new MinioException(error);
    }

    return success;
  }

  private string GetIndexPath(Repository repository) {
    return Path.Join("git", repository.Owner, $"{repository.Name}.index");
  }

  private string GetTimestampPath(Repository repository) {
    return Path.Join("git", repository.Owner, $"{repository.Name}.timestamp");
  }

  private static bool ExecuteGitCommand(string command,
                                        string working_directory = "") {
    ProcessStartInfo psi = new() {
      FileName = "git",
      Arguments = command,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true,
      WorkingDirectory = working_directory
    };

    Process process = new() {
      StartInfo = psi
    };
    process.Start();
    process.WaitForExit();
    return process.ExitCode == 0;
  }
}