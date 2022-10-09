namespace APC.Ingestion;

public class Worker : BackgroundService {
  private readonly ILogger<Worker> _logger;

  public Worker(ILogger<Worker> logger) {
    _logger = logger;
  }


  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    while (!stoppingToken.IsCancellationRequested) await Task.Delay(3000, stoppingToken);
  }
}