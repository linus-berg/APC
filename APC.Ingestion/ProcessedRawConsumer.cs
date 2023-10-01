using APC.Kernel.Messages;
using APC.Kernel.Models;
using APC.Services;
using MassTransit;

namespace APC.Ingestion;

public class ProcessedRawConsumer : IConsumer<ArtifactProcessedRequest> {
  private readonly IArtifactService aps_;
  private readonly IApcCache cache_;
  private readonly IApcDatabase db_;

  public ProcessedRawConsumer(IArtifactService aps, IApcDatabase db,
                              IApcCache cache) {
    db_ = db;
    cache_ = cache;
    aps_ = aps;
  }

  /* On APM returning processed artifact */
  public async Task Consume(ConsumeContext<ArtifactProcessedRequest> context) {
    ArtifactProcessedRequest request = context.Message;
    Artifact artifact = request.Artifact;
    Artifact stored = await db_.GetArtifact(artifact.id, artifact.processor);

    if (await db_.UpdateArtifact(artifact)) {
      await Collect(context);
    }

    if (stored.versions.Count == artifact.versions.Count) {
      /* If version count is the same, end */
      return;
    }


    /* Process all dependencies not already processed in this context */
    HashSet<ArtifactDependency> dependencies = artifact.dependencies;
    foreach (ArtifactDependency dependency in dependencies) {
      if (await cache_.InCache(dependency.id, request.Context)) {
        continue;
      }

      /* Memorize this dependency */
      Artifact dep =
        await aps_.AddArtifact(dependency.id, dependency.processor, "",
                               dependency.config);
      await aps_.Process(dep, request.Context);
    }
  }

  private async Task Collect(ConsumeContext<ArtifactProcessedRequest> context) {
    ArtifactProcessedRequest request = context.Message;
    Artifact artifact = request.Artifact;
    foreach (ArtifactCollectRequest collect in request.CollectRequests) {
      await aps_.Collect(collect);
    }

    await aps_.Route(artifact);
  }
}