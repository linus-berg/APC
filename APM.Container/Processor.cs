using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services.Models;
using APC.Skopeo;
using MassTransit;

namespace APM.Container; 

public class Processor : IProcessor {
  private readonly SkopeoClient skopeo_ = new SkopeoClient();
  public Processor() {
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    ArtifactProcessRequest request = context.Message;
    Artifact artifact = new Artifact() {
      name = request.Name,
      module = request.Module,
    };
    await GetTags(artifact);
    await context.Send(Endpoints.APC_INGEST_PROCESSED, new ArtifactProcessedRequest {
      Context = context.Message.Context,
      Artifact = artifact
    });
  }

  private async Task GetTags(Artifact artifact) {
    SkopeoListTagsOutput list_tags = await skopeo_.GetTags(artifact.name);
    foreach (string tag in list_tags.Tags) {
      ArtifactVersion version = new ArtifactVersion() {
        artifact_id = artifact.id,
        location = $"docker://{artifact.name}:{tag}",
        version = tag
      };
      artifact.AddVersion(version);
    }
  }
}