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
    await SendRequest(Endpoints.S_APC_ACM_ROUTER, new ArtifactRouteRequest {
      artifact = artifact
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
    await SendRequest(Endpoints.S_APC_INGEST_UNPROCESSED,
                      new ArtifactIngestRequest {
                        artifact = artifact
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

  public async Task<bool> Track(string id, string processor_id) {
    Artifact artifact =
      await db_.GetArtifact(id, processor_id);
    if (artifact == null) {
      return false;
    }

    Processor processor = await db_.GetProcessor(processor_id);
    return await Track(artifact, processor);
  }


  public async Task Track() {
    IEnumerable<Processor> processors = await db_.GetProcessors();
    foreach (Processor processor in processors) {
      await Track(processor);
    }
  }

  public async Task Track(string processor_str) {
    Processor processor = await db_.GetProcessor(processor_str);
    await Track(processor);
  }

  public async Task Validate() {
    IEnumerable<Processor> processors = await db_.GetProcessors();
    foreach (Processor processor in processors) {
      try {
        await Validate(processor);
      } catch (Exception e) {
        logger_.LogError("{Error}", e.ToString());
      }
    }
  }

  public async Task Validate(Processor processor) {
    IEnumerable<Artifact>
      artifacts = await db_.GetArtifacts(processor.id, false);
    logger_.LogInformation($"Validating> {processor}={artifacts.Count()}");
    foreach (Artifact artifact in artifacts) {
      await Validate(artifact, processor);
    }
  }

  public async Task Validate(string id, string processor_id) {
    Processor processor = await db_.GetProcessor(processor_id);
    Artifact artifact = await db_.GetArtifact(id, processor_id);
    await Validate(artifact, processor);
  }

  public async Task Validate(Artifact artifact, Processor processor) {
    if (processor.direct_collect) {
      await Collect(artifact.id, processor.id);
    } else {
      await Route(artifact);
    }
  }

  private async Task<bool> Track(Artifact artifact, Processor processor) {
    if (processor.direct_collect) {
      await Collect(artifact.id, artifact.processor);
    } else {
      await Ingest(artifact);
    }

    return true;
  }

  public async Task Track(Processor processor) {
    IEnumerable<Artifact> artifacts = await db_.GetArtifacts(processor.id);
    foreach (Artifact artifact in artifacts) {
      await Track(artifact, processor);
    }
  }

  private async Task SendRequest<T>(Uri uri, T request) {
    ISendEndpoint endpoint = await bus_.GetSendEndpoint(uri);
    if (request != null) {
      await endpoint.Send(request);
    }
  }
}