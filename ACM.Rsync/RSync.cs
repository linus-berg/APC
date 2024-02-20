using APC.Kernel;

namespace ACM.Rsync;

public class RSync {
  private readonly ILogger<RSync> logger_;
  public RSync(ILogger<RSync> logger) {
    logger_ = logger;
  }
  public async Task<bool> Mirror(string remote) {
    /* Bucket is hardcoded to rsync */
    return await Archive(remote, "rsync");
  }

  private async Task<bool> Archive(string remote, string bucket) {
    return await Bin.Execute("rsync-os", args => {
      args.Add(remote);
      args.Add(bucket);
    }, logger_);
  }
}