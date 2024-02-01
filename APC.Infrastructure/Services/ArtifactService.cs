using APC.Kernel;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using APC.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace APC.Infrastructure.Services;

public class ArtifactService : IArtifactService {
  private readonly ISendEndpointProvider bus_;
  private readonly IApcCache cache_;
  private readonly IApcDatabase db_;
  private readonly ILogger<ArtifactService> logger_;

  public ArtifactService(IApcCache cache, IApcDatabase db,
                         ISendEndpointProvider bus,
                         ILogger<ArtifactService> logger) {
    cache_ = cache;
    bus_ = bus;
    db_ = db;
    logger_ = logger;
  }

  public async Task<Artifact> AddArtifact(string id, string processor,
                                          string filter,
                                          Dictionary<string, string> config,
                                          bool root = false) {
    Artifact existing = await db_.GetArtifact(id, processor);
    if (existing != null) {
      return existing;
    }

    Artifact artifact = new();
    artifact.id = id;
    artifact.processor = processor;
    artifact.filter = filter;
    artifact.root = root;
    artifact.config = config;

    await db_.AddArtifact(artifact);
    return artifact;
  }

  public async Task Route(Artifact artifact) {
    await SendRequest(Endpoints.APC_ACM_ROUTER, new ArtifactRouteRequest {
      Artifact = artifact
    });
  }

  public async Task Collect(string location, string processor) {
    ArtifactCollectRequest request = new() {
      location = location,
      module = processor
    };
    await Collect(request);
  }

  public async Task Collect(ArtifactCollectRequest request) {
    await SendRequest(new Uri($"queue:{request.GetCollectorModule()}"),
                      request);
  }

  public async Task Ingest(Artifact artifact) {
    await SendRequest(Endpoints.APC_INGEST_UNPROCESSED,
                      new ArtifactIngestRequest {
                        Artifact = artifact
                      });
  }

  public async Task Process(Artifact artifact, Guid? existing_ctx = null) {
    Guid ctx = existing_ctx ?? await cache_.InitKey(artifact.id);
    if (existing_ctx != null) {
      await cache_.AddToCache(artifact.id, ctx);
    }

    await SendRequest(new Uri($"queue:apm-{artifact.processor}"),
                      new ArtifactProcessRequest {
                        ctx = ctx,
                        artifact = artifact
                      });
  }

  public async Task<bool> Track(string id, string processor) {
    Artifact artifact =
      await db_.GetArtifact(id, processor);
    if (artifact == null) {
      return false;
    }

    if (!artifact.root) {
      return false;
    }

    await Ingest(artifact);
    return true;
  }

  public async Task ReTrack() {
    IEnumerable<Processor> processors = await db_.GetProcessors();
    int proc_count = 0;
    int artifact_count = 0;
    foreach (Processor processor in processors) {
      IEnumerable<Artifact> artifacts = await db_.GetArtifacts(processor.id);
      foreach (Artifact artifact in artifacts) {
        if (processor.direct_collect) {
          await Collect(artifact.id, artifact.processor);
        } else {
          await Ingest(artifact);
        }

        artifact_count++;
      }

      proc_count++;
    }
  }

  public async Task Validate() {
    IEnumerable<Processor> processors = await db_.GetProcessors();
    foreach (Processor processor in processors) {
      try {
        await Validate(processor);
      } catch (Exception e) {
        logger_.LogError(e.ToString());
      }
    }
  }

  public async Task Validate(Processor processor) {
    IEnumerable<Artifact>
      artifacts = await db_.GetArtifacts(processor.id, false);
    logger_.LogInformation($"Validating> {processor}={artifacts.Count()}");
    foreach (Artifact artifact in artifacts) {
      await Route(artifact);
    }
  }

  private async Task SendRequest<T>(Uri uri, T request) {
    ISendEndpoint endpoint = await bus_.GetSendEndpoint(uri);
    await endpoint.Send(request);
  }
}