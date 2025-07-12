using APC.Kernel;
using APC.Kernel.Extensions;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using APC.Skopeo;
using MassTransit;

namespace APM.Container;

public class Processor : IProcessor {
  private readonly SkopeoClient skopeo_;

  public Processor(SkopeoClient skopeo) {
    skopeo_ = skopeo;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    ArtifactProcessRequest request = context.Message;
    Artifact artifact = request.artifact;
    await GetTags(artifact);
    await context.ProcessorReply(artifact);
  }

  private async Task GetTags(Artifact artifact) {
    SkopeoListTagsOutput? list_tags = await skopeo_.GetTags(artifact.id);
    if (list_tags?.Tags != null) {
      foreach (string tag in list_tags.Tags) {
        if (artifact.HasVersion(tag)) {
          continue;
        }

        ArtifactVersion version = new() {
          version = tag
        };
        version.AddFile(
          $"{artifact.id}:{tag}",
          $"docker://{list_tags.Repository}:{tag}"
        );
        artifact.AddVersion(version);
      }
    }
  }
}