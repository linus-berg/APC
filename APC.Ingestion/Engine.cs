using APC.Infrastructure;
using APC.Infrastructure.Models;
using APC.Kernel.Messages;
using MassTransit;

namespace APC.Ingestion;

public class Engine : IConsumer<ArtifactProcessedRequest> {
  private readonly Database db_;

  public Engine(Database db) {
    db_ = db;
  }

  /* On APM returning processed artifact */
  public async Task Consume(ConsumeContext<ArtifactProcessedRequest> context) {
    Artifact artifact = context.Message.Artifact;
    Artifact db_artifact = await db_.GetArtifactByName(artifact.name);

    bool added = false;
    if (db_artifact == null) {
      await db_.AddArtifact(artifact);
      Console.WriteLine($"Added: {artifact.name}");
      added = true;
    }
    
    HashSet<string> deps = added ? artifact.dependencies : artifact.DepDiff(await db_.GetDependencies(artifact.id));
    if (deps.Count == 0 && !added) {
      Console.WriteLine($"No new dependencies: {artifact.name}");
    }
    foreach (string dependency in deps) {
      await context.Send(new Uri($"queue:{artifact.module}-module"), new ArtifactProcessRequest {
        Name = dependency,
        Module = artifact.module
      });
    }
  }
}