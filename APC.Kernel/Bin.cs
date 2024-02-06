using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace APC.Kernel;

public static class Bin {
  public static async Task<bool> Execute(string binary, string args,
                                         string wd = "",
                                         int success_code = 0,
                                         bool use_shell_execute = false,
                                         CancellationToken token =
                                           default) {
    ProcessStartInfo psi = new() {
      FileName = binary,
      Arguments = args,
      WorkingDirectory = wd,
      RedirectStandardOutput = true,
      RedirectStandardError = true,
      UseShellExecute = use_shell_execute,
      CreateNoWindow = true
    };

    Process process = new() {
      StartInfo = psi
    };
    process.Start();
    await process.WaitForExitAsync(token);
    Console.WriteLine(await process.StandardOutput.ReadToEndAsync()); 
    Console.WriteLine(await process.StandardError.ReadToEndAsync()); 
    return process.ExitCode == success_code;
  }
}