using APC.GitUnpack.Models;
using APC.Kernel;

namespace APC.GitUnpack;

public class Unpacker {
  private readonly string in_dir_;
  private readonly string applied_dir_;
  private readonly string repo_dir_;
  public Unpacker() {
    in_dir_ = Environment.GetEnvironmentVariable("GIT_BUNDLE_INPUT");
    applied_dir_ = Environment.GetEnvironmentVariable("GIT_BUNDLE_APPLIED");
    repo_dir_ = Environment.GetEnvironmentVariable("GIT_BUNDLE_REPOS");
  }

  public async Task<int> ProcessBundles() {
    IEnumerable<GitBundle> bundles =
      Directory.GetFiles(in_dir_, "*.bundle", SearchOption.AllDirectories)
               .Select(f => new GitBundle(f));

    IEnumerable<IGrouping<string, GitBundle>> bundle_groups = bundles.GroupBy(b => b.Repository);
    
    foreach (IGrouping<string, GitBundle> bg in bundle_groups) {
      Console.WriteLine(bg.Key);
      
      /* Order by To date */
      IOrderedEnumerable<GitBundle> ordered = bg.OrderBy(gb => gb.To);
      foreach (GitBundle git_bundle in ordered) {
        if (git_bundle.IsFirstBundle) {
          await TryCreateRepository(git_bundle);
        } else {
          await TryApplyBundle(git_bundle);
        }
      }
    }
    return 0;
  }
  
  private async Task<bool> TryApplyBundle(GitBundle bundle) {
    if (!Directory.Exists(bundle.RepositoryDir)) {
      return false;
    }
    
    bool is_valid =
      await Bin.Execute("git", $"bundle verify {bundle.Filepath}",
                        bundle.RepositoryDir);
    if (!is_valid) {
      return false;
    }
    string tmp_file = bundle.MoveToApply();
    await Bin.Execute("git", "fetch --all", bundle.RepositoryDir);
    await UpdateServerInfo(bundle);
    File.Move(tmp_file, Path.Join(applied_dir_, Path.GetFileName(bundle.Filepath)), true);
    //File.Move(tmp_file, bundle.Filepath);
    return true;
  }
  
  private async Task<bool> TryCreateRepository(GitBundle bundle) {
    string tmp_file = bundle.MoveToApply();
    await Bin.Execute("git",
                      $"clone --bare {tmp_file} {bundle.Repository}",
                      repo_dir_);

    await ModifyConfigFile(bundle);
    await UpdateServerInfo(bundle);
    File.Move(tmp_file, Path.Join(applied_dir_, Path.GetFileName(bundle.Filepath)), true);
    //File.Move(tmp_file, bundle.Filepath);
    return false;
  }

  private async Task ModifyConfigFile(GitBundle bundle) {
    string config_file = Path.Join(bundle.RepositoryDir, "config");
    await File.AppendAllLinesAsync(config_file, new[] {
      "\tfetch = +refs/*:refs/*"
    });
  }

  private async Task UpdateServerInfo(GitBundle bundle) {
    await Bin.Execute("git", "update-server-info", bundle.RepositoryDir);
  }
}