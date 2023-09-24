﻿using System.Text;
using APC.Kernel;
using APC.Kernel.Extensions;
using APC.Skopeo.Models;
using CliWrap;

namespace APC.Skopeo;

public class SkopeoClient {
  public async Task<bool> CopyToRegistry(string remote_image) {
    Image image = new(remote_image);
    string registry =
      Configuration.GetApcVar(ApcVariable.ACM_CONTAINER_REGISTRY);

    string internal_image = $"docker://{registry}/{image.Repository}";
    StringBuilder std_out = new StringBuilder();
    StringBuilder std_err = new StringBuilder();
    Command cmd = Cli.Wrap("skopeo")
                     .WithArguments(args => {
                       args.Add("copy");
                       args.Add("--dest-tls-verify=false");
                       args.Add(image.Uri);
                       args.Add(internal_image);
                     })
                     .WithStandardOutputPipe(
                       PipeTarget.ToStringBuilder(std_out))
                     .WithStandardErrorPipe(
                       PipeTarget.ToStringBuilder(std_err));
    Console.WriteLine($"Pulling {image.Uri} -> {internal_image}");
    try {
      CommandResult result =
        await cmd.ExecuteAsync();
    } catch (Exception e) {
      Console.WriteLine(std_err);
      throw;
    }
    return true;
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

  public async Task<SkopeoManifest?> ImageExists(string input) {
    Image image = new(input);
    string registry =
      Configuration.GetApcVar(ApcVariable.ACM_CONTAINER_REGISTRY);
    Command cmd = Cli.Wrap("skopeo")
                     .WithArguments(args => {
                       args.Add("inspect");
                       args.Add("--tls-verify=false");
                       args.Add(
                         $"docker://{registry}/{image.Repository}");
                     });
    SkopeoManifest manifest;
    try {
      manifest = await cmd.ExecuteWithResult<SkopeoManifest>();
    } catch (Exception e) {
      return null;
    }

    return manifest;
  }
}