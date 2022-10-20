using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services.Models;
using MassTransit;

namespace APM.Npm;

public class Processor : IProcessor {
  private readonly INpm npm_;

  public Processor(INpm npm) {
    npm_ = npm;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    string name = context.Message.Name;
    Artifact artifact = await npm_.ProcessArtifact(name);
    await context.Send(Endpoints.APC_INGEST_PROCESSED, new ArtifactProcessedRequest {
      Context = context.Message.Context,
      Artifact = artifact
    });
  }
}