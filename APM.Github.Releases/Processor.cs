using APC.Github;
using APC.Kernel;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Github.Releases;

public class Processor : IProcessor {
  private readonly IGithubReleases gh_;

  public Processor(IGithubReleases gh) {
    gh_ = gh;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await gh_.ProcessArtifact(artifact);
    await context.Send(Endpoints.APC_INGEST_PROCESSED,
                       new ArtifactProcessedRequest {
                         Context = context.Message.ctx,
                         Artifact = artifact
                       });
  }
}
