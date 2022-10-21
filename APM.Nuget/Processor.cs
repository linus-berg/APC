using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services.Models;
using MassTransit;

namespace APM.Nuget;

public class Processor : IProcessor {
  private readonly INuget nuget_;

  public Processor(INuget nuget) {
    nuget_ = nuget;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    string name = context.Message.Name;
    Console.WriteLine($"PROCESSING: {name}");
    Artifact artifact = await nuget_.ProcessArtifact(name);
    await context.Send(Endpoints.APC_INGEST_PROCESSED, new ArtifactProcessedRequest {
      Context = context.Message.Context,
      Artifact = artifact
    });
  }
}
