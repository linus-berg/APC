namespace APC.GitUnpack;

public class Worker : BackgroundService {
  private readonly ILogger<Worker> logger_;
  private readonly Unpacker unpacker_;
  public Worker(ILogger<Worker> logger, Unpacker unpacker) {
    logger_ = logger;
    unpacker_ = unpacker;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    while (!stoppingToken.IsCancellationRequested) {
      await unpacker_.ProcessBundles();
      await Task.Delay(60000, stoppingToken);
    }
  }
}