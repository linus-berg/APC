using APC.Kernel;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Nuget;

public class Processor : IProcessor {
  private readonly INuget nuget_;

  public Processor(INuget nuget) {
    nuget_ = nuget;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await nuget_.ProcessArtifact(artifact);
    await context.Send(Endpoints.APC_INGEST_PROCESSED,
                       new ArtifactProcessedRequest {
                         Context = context.Message.ctx,
                         Artifact = artifact
                       });
  }
}