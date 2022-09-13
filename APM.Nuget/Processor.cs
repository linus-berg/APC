using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace APM.Nuget;

public class Processor : IProcessor {
  public Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    string name = context.Message.Name;
    Console.WriteLine($"PROCESSING: {name}");
    return Task.CompletedTask;
  }
}