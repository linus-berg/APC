using APC.Kernel;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Jetbrains.IDE;

public class Processor : IProcessor {
  private readonly IJetbrains jetbrains_;

  public Processor(IJetbrains jetbrains) {
    jetbrains_ = jetbrains;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await jetbrains_.ProcessArtifact(artifact);
    await context.Send(Endpoints.APC_INGEST_PROCESSED,
                       new ArtifactProcessedRequest {
                         Context = context.Message.ctx,
                         Artifact = artifact
                       });
  }
}