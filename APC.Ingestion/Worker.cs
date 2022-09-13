using APC.Kernel.Messages;
using MassTransit;

namespace APC.Ingestion;

public class Worker : BackgroundService {
  private readonly ILogger<Worker> _logger;
  private readonly IBus bus_;

  public Worker(ILogger<Worker> logger, IBus bus) {
    _logger = logger;
    bus_ = bus;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    ISendEndpoint endpoint = await bus_.GetSendEndpoint(new Uri("queue:npm-module"));
    ArtifactProcessRequest apr = new();
    apr.Name = "react";
    apr.Module = "npm";
    await endpoint.Send(apr);
    while (!stoppingToken.IsCancellationRequested) {
      _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
      await Task.Delay(3000, stoppingToken);
    }
  }
}