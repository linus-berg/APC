using System.Text;
using CliWrap;
using CliWrap.Builders;
using Microsoft.Extensions.Logging;

namespace APC.Kernel;

public static class Bin {
  public static async Task<bool> Execute(string binary,
                                         Action<ArgumentsBuilder> builder,
                                         ILogger logger,
                                         string wd = "",
                                         int success_code = 0,
                                         CancellationToken token =
                                           default) {
    StringBuilder std_out = new();
    StringBuilder std_err = new();
    Command cmd = Cli.Wrap(binary)
                     .WithArguments(builder)
                     .WithWorkingDirectory(wd)
                     .WithStandardOutputPipe(
                       PipeTarget.ToStringBuilder(std_out))
                     .WithStandardErrorPipe(
                       PipeTarget.ToStringBuilder(std_err));
    CommandResult? result = null;
    try {
      result = await cmd.ExecuteAsync(token);
    } catch (Exception e) {
      logger.LogError("{Error}", e.ToString());
    }

    logger.LogDebug("{StdOut}", std_out.ToString());
    logger.LogDebug("{StdErr}", std_err.ToString());
    return result?.ExitCode == success_code;
  }
}