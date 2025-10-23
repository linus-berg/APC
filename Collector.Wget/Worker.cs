namespace Collector.Wget;

public class Worker : BackgroundService {
  public Worker(ILogger<Worker> logger) {
  }

  protected override async Task ExecuteAsync(CancellationToken stopping_token) {
    while (!stopping_token.IsCancellationRequested) {
      await Task.Delay(1000, stopping_token);
    }
  }
}