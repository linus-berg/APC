using System.Diagnostics;

namespace ACM.Rsync; 

public class RSync {
  
  public async Task<bool> Mirror(string remote) {
    /* Bucket is hardcoded to rsync */
    return await Archive(remote, "rsync");
  }

  private async Task<bool> Archive(string remote, string bucket) {
    return await ExecuteRsyncCommand($"{remote} ${bucket}");
  }
  
  private static async Task<bool> ExecuteRsyncCommand(string command) {
    /* rsync-os is a go implementation of rsync allowing syncing to S3. */
    ProcessStartInfo psi = new() {
      FileName = "rsync-os",
      Arguments = command,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true,
    };

    Process process = new() {
      StartInfo = psi
    };
    process.Start();
    await process.WaitForExitAsync();
    return process.ExitCode == 0;
  }
}