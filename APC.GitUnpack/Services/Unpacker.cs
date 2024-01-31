using APC.GitUnpack.Models;
using APC.Kernel;

namespace APC.GitUnpack.Services;

public class Unpacker {
  private readonly string archive_dir_;
  private readonly string in_dir_;
  private readonly string repo_dir_;

  /* TODO: This class should probably be cleaned up */
  public Unpacker() {
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
               .Select(f => new GitBundle(f));

    IEnumerable<IGrouping<string, GitBundle>> bundle_groups =
      bundles.GroupBy(b => b.Repository);

    foreach (IGrouping<string, GitBundle> bg in bundle_groups) {
      /* Order by To date */
      IOrderedEnumerable<GitBundle> ordered = bg.OrderBy(gb => gb.To);
      foreach (GitBundle git_bundle in ordered) {
        await TryApplyBundle(git_bundle, token);
      }
    }

    return 0;
  }

  private async Task<bool> TryApplyBundle(GitBundle bundle,
                                          CancellationToken token = default) {
    bool is_valid = await CheckIfValid(bundle, token);
    if (!is_valid) {
      return false;
    }

    string tmp_file = bundle.MoveToApply();
    if (bundle.IsFirstBundle) {
      await Bin.Execute("git",
                        $"clone --bare {tmp_file} {bundle.Repository}",
                        repo_dir_, 0, token);
    } else {
      await Bin.Execute("git", "fetch --all", bundle.RepositoryDir, 0, token);
    }

    await Cleanup(tmp_file, bundle);
    return true;
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
                        bundle.RepositoryDir, 0, token);
    return is_valid;
  }

  private async Task Cleanup(string file, GitBundle bundle) {
    /* Add the necessary configurations */
    if (bundle.IsFirstBundle) {
      await ModifyConfigFile(bundle);
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
    await Bin.Execute("git", "update-server-info", bundle.RepositoryDir, 0,
                      token);
  }

  private void MoveToArchive(string file, GitBundle bundle) {
    File.Move(file,
              Path.Join(archive_dir_, Path.GetFileName(bundle.Filepath)), true);
  }
}