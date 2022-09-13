using APC.Infrastructure.Models;
using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace APM.Npm;

public class Processor : IProcessor {
  private readonly INpm npm_;

  public Processor(INpm npm) {
    npm_ = npm;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    string name = context.Message.Name;
    Console.WriteLine($"PROCESSING: {name}");
    Artifact artifact = await npm_.ProcessArtifact(name);
    await context.Send(Endpoints.APC_INGEST, new ArtifactProcessedRequest {
      Artifact = artifact
    });
  }
}