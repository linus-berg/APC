using APC.Kernel.Messages;
using APC.Kernel.Models;
using APC.Services;
using MassTransit;

namespace APC.Ingestion;

public class ProcessedRawConsumer : IConsumer<ArtifactProcessedRequest> {
  private readonly IArtifactService aps_;
  private readonly IApcCache cache_;
  private readonly IApcDatabase db_;
  private readonly ILogger<ProcessedRawConsumer> logger_;

  public ProcessedRawConsumer(IArtifactService aps, IApcDatabase db,
                              IApcCache cache,
                              ILogger<ProcessedRawConsumer> logger) {
    db_ = db;
    cache_ = cache;
    aps_ = aps;
    logger_ = logger;
  }

  /* On APM returning processed artifact */
  public async Task Consume(ConsumeContext<ArtifactProcessedRequest> context) {
    ArtifactProcessedRequest request = context.Message;
    Artifact artifact = request.artifact;
    Artifact stored = await db_.GetArtifact(artifact.id, artifact.processor);

    if (await db_.UpdateArtifact(artifact)) {
      /* Collecting artifact files due to artifact being updated */
      await Collect(context);
      logger_.LogInformation("ARTIFACT:UPDATED:{ArtifactId}", artifact.id);
    }

    if (stored.versions.Count == artifact.versions.Count &&
        stored.dependencies.Count == artifact.dependencies.Count) {
      /* If version count is the same, end */
      return;
    }


    /* Process all dependencies not already processed in this context */
    HashSet<ArtifactDependency> dependencies = artifact.dependencies;
    foreach (ArtifactDependency dependency in dependencies) {
      if (await cache_.InCache(dependency.id, request.context)) {
        continue;
      }

      /* Memorize this dependency */
      Artifact dep =
        await aps_.AddArtifact(
          dependency.id,
          dependency.processor,
          "",
          dependency.config
        );
      await aps_.Process(dep, request.context);
    }
  }

  private async Task Collect(ConsumeContext<ArtifactProcessedRequest> context) {
    ArtifactProcessedRequest request = context.Message;
    Artifact artifact = request.artifact;
    foreach (ArtifactCollectRequest collect in request.collect_requests) {
      await aps_.Collect(collect);
    }

    await aps_.Route(artifact);
  }
}