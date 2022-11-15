using System.Text;
using System.Text.Json;
using CliWrap;

namespace APC.Skopeo;

public class SkopeoClient {
  public async Task CopyToOci(string image, string oci_dir) {
    Uri uri = new(image);
    Directory.CreateDirectory(oci_dir);
    Command cmd = Cli.Wrap("skopeo").WithWorkingDirectory(oci_dir)
                     .WithArguments(args => {
                       args.Add("copy");
                       args.Add("--quiet");
                       args.Add("--dest-shared-blob-dir");
                       args.Add("shared");
                       args.Add(image);
                       args.Add(
                         $"oci:repo:{uri.GetComponents(UriComponents.Host | UriComponents.Path, UriFormat.Unescaped)}");
                     });
    StringBuilder sb = new();
    Console.WriteLine($"Pulling {image}");
    try {
      CommandResult result =
        await (cmd | PipeTarget.ToStringBuilder(sb)).ExecuteAsync();
      Console.WriteLine(sb);
    } catch (Exception e) {
      Console.WriteLine(e);
      throw;
    }
  }

  public async Task<SkopeoListTagsOutput> GetTags(string image) {
    Command cmd = Cli.Wrap("skopeo").WithArguments(args => {
      args.Add("list-tags");
      args.Add($"docker://{image}");
    });
    return await MapCommandOutput<SkopeoListTagsOutput>(cmd);
  }

  private async Task<T> MapCommandOutput<T>(Command cmd) {
    StringBuilder sb = new();
    CommandResult result =
      await (cmd | PipeTarget.ToStringBuilder(sb)).ExecuteAsync();

    if (result.ExitCode != 0) {
      throw new ApplicationException("Skopeo failed");
    }

    return JsonSerializer.Deserialize<T>(sb.ToString());
  }
}