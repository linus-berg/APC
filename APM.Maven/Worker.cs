namespace APM.Maven;

public class Worker : BackgroundService {
  private readonly ILogger<Worker> logger_;

  public Worker(ILogger<Worker> logger) {
    logger_ = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stopping_token) {
    while (!stopping_token.IsCancellationRequested) {
      logger_.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
      await Task.Delay(1000, stopping_token);
    }
  }
}