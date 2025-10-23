using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using Library.Skopeo;
using MassTransit;

namespace Processor.Container;

public class Consumer : IProcessor {
  private readonly SkopeoClient skopeo_;

  public Consumer(SkopeoClient skopeo) {
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