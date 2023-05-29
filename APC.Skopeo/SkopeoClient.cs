using System.Text;
using APC.Kernel.Extensions;
using APC.Skopeo.Models;
using CliWrap;

namespace APC.Skopeo;

public class SkopeoClient {
  public async Task CopyToOci(string input, string oci_dir) {
    Image image = new(input);
    OciDir oci = new(oci_dir);
    Directory.CreateDirectory(oci.Repositories);
    Directory.CreateDirectory(Path.Join(oci.Repositories, image.Destination));
    Command cmd = Cli.Wrap("skopeo")
                     .WithWorkingDirectory(oci.Repositories)
                     .WithArguments(args => {
                       args.Add("copy");
                       args.Add("--quiet");
                       args.Add("--dest-shared-blob-dir");
                       args.Add(oci.Shared);
                       args.Add(image.Uri);
                       args.Add(
                         $"oci:{image.Repository}");
                     });
    StringBuilder sb = new();
    Console.WriteLine($"Pulling {image.Uri} -> oci:{image.Repository}");
    try {
      CommandResult result =
        await (cmd | PipeTarget.ToStringBuilder(sb)).ExecuteAsync();
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
    Directory.CreateDirectory(oci.Repositories);
    if (!Directory.Exists(Path.Join(oci.Repositories, image.Destination))) {
      return null;
    }
    Command cmd = Cli.Wrap("skopeo")
                     .WithWorkingDirectory(oci.Repositories)
                     .WithArguments(args => {
                       args.Add("inspect");
                       args.Add("--shared-blob-dir");
                       args.Add(oci.Shared);
                       args.Add(
                         $"oci:{image.Repository}");
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