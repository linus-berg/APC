using APC.Infrastructure;
using APC.Kernel.Messages;
using MassTransit;

namespace APC.Ingestion;

public class Worker : BackgroundService {
  private readonly ILogger<Worker> _logger;
  private readonly IBus bus_;
  private readonly RedisCache cache_;

  public Worker(ILogger<Worker> logger, IBus bus, RedisCache cache) {
    _logger = logger;
    bus_ = bus;
    cache_ = cache;
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

  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    /* Run as init */
    await GetArtifact("react", "npm", stoppingToken);
    await GetArtifact("HotChocolate.AspNetCore", "nuget", stoppingToken);
    
    while (!stoppingToken.IsCancellationRequested) {
      _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
      await Task.Delay(3000, stoppingToken);
    }
  }
}