using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services;
using APC.Services.Models;
using MassTransit;

namespace APC.Ingestion;

public class ProcessedConsumer : IConsumer<ArtifactProcessedRequest> {
  private readonly IApcCache cache_;
  private readonly IApcDatabase db_;

  public ProcessedConsumer(IApcDatabase db, IApcCache cache) {
    db_ = db;
    cache_ = cache;
  }

  /* On APM returning processed artifact */
  public async Task Consume(ConsumeContext<ArtifactProcessedRequest> context) {
    ArtifactProcessedRequest request = context.Message;
    Artifact artifact = request.Artifact;

    /* If not in db add */
    if (await TryInsertOrUpdateArtifact(artifact)) {
      await Collect(context);
    } else {
      return;
    }


    /* Process all dependencies not already processed in this context */
    HashSet<ArtifactDependency> dependencies = artifact.dependencies;
    foreach (ArtifactDependency dependency in dependencies) {
      if (await cache_.InCache(dependency.name, request.Context)) {
        continue;
      }

      /* Memorize this dependency */
      await cache_.AddToCache(dependency.name, request.Context);
      await Process(context, dependency);
    }
  }

  private async Task<bool> TryInsertOrUpdateArtifact(Artifact artifact) {
    Artifact db_artifact =
      await db_.GetArtifactByName(artifact.id, artifact.module);
    try {
      if (db_artifact != null) {
        artifact.filter = db_artifact.filter;
        return await db_.UpdateArtifact(artifact);
      }

      await db_.AddArtifact(artifact);
      return true;
    } catch (Exception e) {
      Console.WriteLine($"{artifact.id}->{e.Message}");
      return true;
    }
  }

  private async Task Collect(ConsumeContext<ArtifactProcessedRequest> context) {
    ArtifactProcessedRequest request = context.Message;
    Artifact artifact = request.Artifact;
    foreach (ArtifactCollectRequest collect in request.CollectRequests) {
      await context.Send(new Uri($"queue:{collect.GetCollectorModule()}"),
                         collect);
    }

    await context.Send(Endpoints.APC_ACM_ROUTER, new ArtifactRouteRequest {
      Artifact = artifact
    });
  }

  private async Task Process(ConsumeContext<ArtifactProcessedRequest> context,
                             ArtifactDependency dependency) {
    ArtifactProcessedRequest request = context.Message;
    Artifact artifact = request.Artifact;
    await context.Send(new Uri($"queue:apm-{artifact.module}"),
                       new ArtifactProcessRequest {
                         Context = request.Context,
                         Name = dependency.name,
                         Module = dependency.module
                       });
  }
}