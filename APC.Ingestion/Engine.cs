using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APC.Ingestion; 

public class Engine : IConsumer<ArtifactProcessedRequest> {
  HashSet<string> processed = new HashSet<string>();
  public async Task Consume(ConsumeContext<ArtifactProcessedRequest> context) {
    Artifact artifact = context.Message.Artifact;
    Console.WriteLine($"Processed {artifact.Id}");
    foreach (KeyValuePair<string, ArtifactVersion> version in artifact.Versions) {
      foreach (string dep in version.Value.Dependencies) {
        if (processed.Contains(dep)) {
          continue;
        } 
        processed.Add(dep);
        await context.Send(new Uri($"queue:{artifact.Type}-module"), new ArtifactProcessRequest() {
          Artifact = new Artifact() {
            Id = dep,
            Type = artifact.Type
          }
        }); 
      }
    }
  }
}