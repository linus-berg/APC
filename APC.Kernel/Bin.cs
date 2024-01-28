using System.Diagnostics;

namespace APC.Kernel;

public static class Bin {
  public static async Task<bool> Execute(string binary, string args,
                                         string wd = "",
                                         int success_code = 0,
                                         CancellationToken token =
                                           default) {
    ProcessStartInfo psi = new() {
      FileName = binary,
      Arguments = args,
      WorkingDirectory = wd,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = false,
      CreateNoWindow = true
    };

    Process process = new() {
      StartInfo = psi
    };
    process.Start();
    await process.WaitForExitAsync(token);
    return process.ExitCode == success_code;
  }
}