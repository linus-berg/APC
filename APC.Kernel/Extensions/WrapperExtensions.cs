using System.Text;
using System.Text.Json;
using CliWrap;

namespace APC.Kernel.Extensions;

public static class WrapperExtensions {
  public static async Task<T> ExecuteWithResult<T>(this Command cmd) {
    StringBuilder sb = new();
    StringBuilder sb_err = new();
    Command final_cmd =
      cmd | (PipeTarget.ToStringBuilder(sb),
             PipeTarget.ToStringBuilder(sb_err));
    try {
      await final_cmd.ExecuteAsync();
    } catch (Exception e) {
      throw new ApplicationException(sb_err.ToString());
    }

    return JsonSerializer.Deserialize<T>(sb.ToString());
  }

  public static async Task<bool> ExecuteToConsole(this Command cmd) {
    await using Stream stdout = Console.OpenStandardOutput();
    await using Stream stderr = Console.OpenStandardError();
    cmd |= (stdout, stderr);
    try {
      CommandResult result = await cmd.ExecuteAsync();
      return result.ExitCode == 0;
    } catch (Exception e) {
      return false;
    }
  }
}