using System.Threading.Tasks;
using ACM.Kernel;
using APC.Kernel;

namespace ACM.Wget;

public class Wget {
  private readonly FileSystem fs_;
  private readonly string wd_;

  public Wget(FileSystem fs) {
    fs_ = fs;
    wd_ = fs_.GetModuleDir("wget", true);
  }

  public async Task<bool> Mirror(string remote) {
    return await Archive(remote);
  }

  private async Task<bool> Archive(string remote) {
    return await Bin.Execute("wget", $"--mirror -k -p -E --no-parent {remote}", wd_);
  }
}