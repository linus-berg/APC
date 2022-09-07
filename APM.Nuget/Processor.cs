using APC.Kernel;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Nuget;

public class Processor : IProcessor {
  public Processor() {
  }

  public Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.Artifact;
    Console.WriteLine($"PROCESSING: {artifact.Id}");
    return Task.CompletedTask;
  }
}