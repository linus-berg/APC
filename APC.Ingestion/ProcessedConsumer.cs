using APC.Infrastructure;
using APC.Infrastructure.Models;
using APC.Kernel.Messages;
using MassTransit;

namespace APC.Ingestion;

public class Engine : IConsumer<ArtifactProcessedRequest> {
  private readonly RedisCache cache_;
  private readonly Database db_;

  public Engine(Database db, RedisCache cache) {
    db_ = db;
    cache_ = cache;
  }

  /* On APM returning processed artifact */
  public async Task Consume(ConsumeContext<ArtifactProcessedRequest> context) {
    ArtifactProcessedRequest request = context.Message;
    Artifact artifact = request.Artifact;

    Artifact db_artifact = await db_.GetArtifactByName(artifact.name, artifact.module);

    /* If not in db add */
    bool updated;
    if (db_artifact == null) {
      await db_.AddArtifact(artifact);
      Console.WriteLine($"Added: {artifact.name}");
      updated = true;
    }
    else {
      updated = await db_.UpdateArtifact(db_artifact, artifact);
    }

    if (updated) {
      await db_.Commit();
      await Collect(context);
    }
    else {
      return;
    }

    /* Process all dependencies not already processed in this context */
    HashSet<string> dependencies = artifact.dependencies;
    foreach (string dependency in dependencies) {
      if (await cache_.InCache(dependency, request.Context)) {
        Console.WriteLine($"Hit cache {artifact.name}:{dependency}");
        continue;
      }

      await cache_.AddToCache(dependency, request.Context);
      Console.WriteLine($"Found new dependency {dependency}");
      /* Memorize this dependency */
      await Process(context, dependency);
    }
  }

  private async Task Collect(ConsumeContext<ArtifactProcessedRequest> context) {
    ArtifactProcessedRequest request = context.Message;
    Artifact artifact = request.Artifact;
    await context.Send(new Uri("queue:acm-http"), new ArtifactCollectRequest {
      Artifact = artifact
    });
  }

  private async Task Process(ConsumeContext<ArtifactProcessedRequest> context, string artifact_name) {
    ArtifactProcessedRequest request = context.Message;
    Artifact artifact = request.Artifact;
    await context.Send(new Uri($"queue:apm-{artifact.module}"), new ArtifactProcessRequest {
      Context = request.Context,
      Name = artifact_name,
      Module = artifact.module
    });
  }
}