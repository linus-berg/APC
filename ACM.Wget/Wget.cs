using System.Diagnostics;
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
    return await ExecuteCommand($"--mirror {remote}");
  }
  
  private async Task<bool> ExecuteCommand(string command) {
    ProcessStartInfo psi = new() {
      FileName = "wget",
      Arguments = command,
      WorkingDirectory = wd_,
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