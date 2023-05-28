using System.Text;
using APC.Kernel.Extensions;
using APC.Skopeo.Models;
using CliWrap;

namespace APC.Skopeo;

public class SkopeoClient {
  public async Task CopyToOci(string input, string oci_dir) {
    Image image = new(input);
    OciDir oci = new(oci_dir);
    string root = oci.GetImageRoot(image);
    Directory.CreateDirectory(root);
    Command cmd = Cli.Wrap("skopeo")
                     .WithWorkingDirectory(root)
                     .WithArguments(args => {
                       args.Add("copy");
                       args.Add("--quiet");
                       args.Add("--dest-shared-blob-dir");
                       args.Add(oci.Shared);
                       args.Add(image.Uri);
                       args.Add(
                         $"oci:{image.Name}:{image.Reference}");
                     });
    StringBuilder sb = new();
    Console.WriteLine($"Pulling {image.Uri}");
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

  public async Task<SkopeoManifest?> ImageExists(string input, string oci_dir) {
    Image image = new(input);
    OciDir oci = new(oci_dir);
    string root = oci.GetImageRoot(image);
    Directory.CreateDirectory(root);
    Command cmd = Cli.Wrap("skopeo")
                     .WithWorkingDirectory(root)
                     .WithArguments(args => {
                       args.Add("inspect");
                       args.Add("--shared-blob-dir");
                       args.Add(oci.Shared);
                       args.Add(
                         $"oci:{image.Name}:{image.Reference}");
                     });
    SkopeoManifest manifest;
    try {
      manifest = await cmd.ExecuteWithResult<SkopeoManifest>();
      manifest.WorkingDirectory = oci_dir;
    } catch (Exception e) {
      return null;
    }

    return manifest;
  }
}