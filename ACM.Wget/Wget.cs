using System.Threading.Tasks;
using ACM.Kernel;
using APC.Kernel;

namespace ACM.Wget;

public class Wget {
  private readonly FileSystem fs_;
  private readonly ILogger<Wget> logger_;
  private readonly string wd_;

  public Wget(ILogger<Wget> logger, FileSystem fs) {
    logger_ = logger;
    fs_ = fs;
    wd_ = fs_.GetModuleDir("wget", true);
  }

  public async Task<bool> Mirror(string remote) {
    return await Archive(remote);
  }

  private async Task<bool> Archive(string remote) {

    return await Bin.Execute("wget",
                             args => {
                               args.Add("--mirror");
                               args.Add("-k");
                               args.Add("-p");
                               args.Add("-E");
                               args.Add("--no-parent");
                               args.Add(remote);
                             }, logger_, wd_);
  }
}