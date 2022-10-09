using APC.Infrastructure;
using APC.Kernel.Messages;
using MassTransit;

namespace APC.Ingestion;

public class IngestConsumer : IConsumer<ArtifactIngestRequest> {
  private readonly ILogger<Worker> _logger;
  private readonly IBus bus_;
  private readonly RedisCache cache_;

  public IngestConsumer(ILogger<Worker> logger, IBus bus, RedisCache cache) {
    _logger = logger;
    bus_ = bus;
    cache_ = cache;
  }

  public async Task Consume(ConsumeContext<ArtifactIngestRequest> context) {
    /* Run as init */
    ArtifactIngestRequest request = context.Message;
    foreach (string artifact in request.Artifacts)
      await GetArtifact(artifact, request.Module, context.CancellationToken);
  }

  private async Task<bool> GetArtifact(string artifact, string module, CancellationToken ct) {
    ArtifactProcessRequest apr = new();
    apr.Name = artifact;
    apr.Module = module;
    apr.Context = await cache_.InitKey(apr.Name);
    ISendEndpoint endpoint = await bus_.GetSendEndpoint(new Uri($"queue:apm-{module}"));
    await endpoint.Send(apr, ct);
    return true;
  }
}