using APC.Kernel.Messages;
using APC.Kernel.Models;
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
    ISendEndpoint nuget_endpoint = await bus_.GetSendEndpoint(new Uri("queue:nuget-module"));
    ArtifactProcessRequest apr = new ArtifactProcessRequest();
    apr.Artifact = new Artifact() {
      Id = "react",
      Type = "npm"
    };
    ArtifactProcessRequest apr_n = new ArtifactProcessRequest();
    apr_n.Artifact = new Artifact() {
      Id = "nuget",
      Type = "nuget"
    };
    await endpoint.Send(apr);
    await nuget_endpoint.Send(apr_n);
    while (!stoppingToken.IsCancellationRequested) {
      _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
      await Task.Delay(3000, stoppingToken);
    }
  }
}