using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services.Models;
using APC.Skopeo;
using MassTransit;

namespace APM.Container;

public class Processor : IProcessor {
  private readonly SkopeoClient skopeo_ = new();

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    ArtifactProcessRequest request = context.Message;
    Artifact artifact = new() {
      id = request.Name,
      module = request.Module
    };
    await GetTags(artifact);
    await context.Send(Endpoints.APC_INGEST_PROCESSED,
                       new ArtifactProcessedRequest {
                         Context = context.Message.Context,
                         Artifact = artifact
                       });
  }

  private async Task GetTags(Artifact artifact) {
    SkopeoListTagsOutput list_tags = await skopeo_.GetTags(artifact.id);
    foreach (string tag in list_tags.Tags) {
      ArtifactVersion version = new() {
        location = $"docker://{artifact.id}:{tag}",
        version = tag
      };
      artifact.AddVersion(version);
    }
  }
}