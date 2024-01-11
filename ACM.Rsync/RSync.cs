using System.Diagnostics;
using APC.Kernel;

namespace ACM.Rsync; 

public class RSync {
  
  public async Task<bool> Mirror(string remote) {
    /* Bucket is hardcoded to rsync */
    return await Archive(remote, "rsync");
  }

  private async Task<bool> Archive(string remote, string bucket) {
    return await Bin.Execute("rsync-os", $"{remote} ${bucket}");
  }
}