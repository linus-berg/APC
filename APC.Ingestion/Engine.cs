using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APC.Ingestion; 

public class Engine : IConsumer<ArtifactProcessedRequest> {

  public Task Consume(ConsumeContext<ArtifactProcessedRequest> context) {
    Artifact artifact = context.Message.Artifact;
    foreach (KeyValuePair<string, ArtifactVersion> version in artifact.Versions) {
      foreach (string dep in version.Value.Dependencies) {
        Console.WriteLine(dep);
      }
    }
    return Task.CompletedTask;
  }
}