using System.Data;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using APC.Kernel.Extensions;
using CliWrap;

namespace APC.Skopeo;

public class SkopeoClient {
  public async Task CopyToOci(string image, string oci_dir) {
    Directory.CreateDirectory(oci_dir);
    Command cmd = Cli.Wrap("skopeo").WithWorkingDirectory(oci_dir)
                     .WithArguments(args => {
                       args.Add("copy");
                       args.Add("--quiet");
                       args.Add("--dest-shared-blob-dir");
                       args.Add("shared");
                       args.Add(image);
                       args.Add(
                         $"oci:repo:{GetImageRef(image)}");
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

  public async Task<SkopeoListTagsOutput?> GetTags(string image) {
    Command cmd = Cli.Wrap("skopeo").WithArguments(args => {
      args.Add("list-tags");
      args.Add($"docker://{image}");
    });
    SkopeoListTagsOutput tags;
    try {
      tags = await cmd.ExecuteWithResult<SkopeoListTagsOutput>();
    } catch (Exception e) {
      Console.WriteLine(e);
      return null;
    }
    return tags;
  }
  
  public async Task<SkopeoManifest?> ImageExists(string image, string oci_dir) {
    Command cmd = Cli.Wrap("skopeo").WithWorkingDirectory(oci_dir)
                     .WithArguments(args => {
                       args.Add("inspect");
                       args.Add("--shared-blob-dir");
                       args.Add("shared");
                       args.Add(
                         $"oci:repo:{GetImageRef(image)}");
                     });
    SkopeoManifest manifest;
    try {
      manifest = await cmd.ExecuteWithResult<SkopeoManifest>();
      manifest.WorkingDirectory = oci_dir;
    } catch (Exception e) {
      Console.WriteLine(e);
      return null;
    }
    return manifest;
  }

  private string GetImageRef(string image) {
    Uri uri = new(image); 
    return uri.GetComponents(UriComponents.Host | UriComponents.Path, UriFormat.Unescaped);
  }
}