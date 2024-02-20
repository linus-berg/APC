using APC.GitUnpack.Models;
using APC.Kernel;

namespace APC.GitUnpack.Services;

public class Unpacker {
  private readonly string archive_dir_;
  private readonly string in_dir_;
  private readonly string repo_dir_;
  private readonly ILogger<Unpacker> logger_;

  /* TODO: This class should probably be cleaned up */
  public Unpacker(ILogger<Unpacker> logger) {
    logger_ = logger;
    in_dir_ = Environment.GetEnvironmentVariable("GIT_BUNDLE_INPUT") ??
              throw new InvalidOperationException("GIT_BUNDLE_INPUT not set.");
    archive_dir_ = Environment.GetEnvironmentVariable("GIT_BUNDLE_ARCHIVE") ??
                   throw new InvalidOperationException(
                     "GIT_BUNDLE_ARCHIVE not set.");
    repo_dir_ = Environment.GetEnvironmentVariable("GIT_BUNDLE_REPOS") ??
                throw new InvalidOperationException(
                  "GIT_BUNDLE_REPOS not set.");
  }

  public async Task<int> ProcessBundles(CancellationToken token) {
    IEnumerable<GitBundle> bundles =
      Directory.GetFiles(in_dir_, "*.bundle", SearchOption.AllDirectories)
               .Select(f => {
                 string relative_path = Path.GetRelativePath(in_dir_, f);
                 string? directory = Path.GetDirectoryName(relative_path);
                 if (string.IsNullOrEmpty(directory)) {
                   throw new ArgumentException(
                     $"Could not get directory of ${relative_path}");
                 }
                 return new GitBundle(f, directory);
               });

    IEnumerable<IGrouping<string, GitBundle>> bundle_groups =
      bundles.GroupBy(b => b.Repository);

    foreach (IGrouping<string, GitBundle> bg in bundle_groups) {
      /* Order by To date */
      IOrderedEnumerable<GitBundle> ordered = bg.OrderBy(gb => gb.To);
      foreach (GitBundle git_bundle in ordered) {
        if (Path.GetFileNameWithoutExtension(git_bundle.Filepath)
                .StartsWith(".")) {
          continue;
        }

        try {
          if (!await TryApplyBundle(git_bundle, token)) {
            ResetToInput(git_bundle);
          }
        } catch (Exception e) {
          logger_.LogError($"Failed to apply {git_bundle.Filepath}: {e}");      
          ResetToInput(git_bundle);      
        }
      }
    }

    return 0;
  }

  private async Task<bool> TryApplyBundle(GitBundle bundle,
                                          CancellationToken token = default) {
    logger_.LogInformation($"Processing: {bundle.Filepath}");
    bool is_valid = await CheckIfValid(bundle, token);
    if (!is_valid) {
      return false;
    }

    string tmp_file = bundle.MoveToApply();
    bool success = false;
    if (bundle.IsFirstBundle) {
      string dir = Path.Join(repo_dir_, bundle.Owner);
      Directory.CreateDirectory(dir);
      success = await Bin.Execute("git",
                        $"clone --mirror {tmp_file} {bundle.Repository}",
                        dir, 0, false, token);
    } else {
      success = await Bin.Execute("git", "remote update --prune", bundle.RepositoryDir, 0, false, token);
    }
    if (success) {
      await Cleanup(tmp_file, bundle);
    }
    return success;
  }

  private async Task<bool> CheckIfValid(GitBundle bundle,
                                        CancellationToken token = default) {
    if (bundle.IsFirstBundle) {
      return true;
    }

    if (!Directory.Exists(bundle.RepositoryDir)) {
      return false;
    }

    /* If incremental bundle validate bundle */
    bool is_valid =
      await Bin.Execute("git", $"bundle verify {bundle.Filepath}",
                        bundle.RepositoryDir, 0, false, token);
    return is_valid;
  }

  private async Task Cleanup(string file, GitBundle bundle) {
    /* Add the necessary configurations */
    if (bundle.IsFirstBundle) {
      /* Not needed anymore ? */
      //await ModifyConfigFile(bundle);
    }
    if (!Directory.Exists(bundle.RepositoryDir)) {
      return;
    }

    await UpdateServerInfo(bundle);
    MoveToArchive(file, bundle);
  }

  private async Task ModifyConfigFile(GitBundle bundle) {
    /* Modify config, ensure this only happens ONCE */
    string config_file = Path.Join(bundle.RepositoryDir, "config");
    await File.AppendAllLinesAsync(config_file, new[] {
      "\tfetch = +refs/*:refs/*"
    });
  }

  private async Task UpdateServerInfo(GitBundle bundle,
                                      CancellationToken token = default) {
    await Bin.Execute("git", "update-server-info", bundle.RepositoryDir, 0, false,
                      token);
  }

  private void MoveToArchive(string file, GitBundle bundle) {
    string dir = Path.Join(archive_dir_, bundle.Owner);
    Directory.CreateDirectory(dir);
    File.Move(file,
              Path.Join(archive_dir_, bundle.Owner,
                        Path.GetFileName(bundle.Filepath)), true);
  }

  private void ResetToInput(GitBundle bundle) {
    try {
      string tmp_file = Path.Join(Path.GetDirectoryName(bundle.Filepath),
                                  bundle.Repository);
      if (File.Exists(tmp_file)) {
        File.Move(tmp_file, bundle.Filepath);
      }
    } catch (Exception e) {
      logger_.LogError(e.ToString());
    }
  }
}