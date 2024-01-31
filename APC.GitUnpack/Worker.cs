using APC.GitUnpack.Services;

namespace APC.GitUnpack;

public class Worker : BackgroundService {
  private readonly ILogger<Worker> logger_;
  private readonly Unpacker unpacker_;

  public Worker(ILogger<Worker> logger, Unpacker unpacker) {
    logger_ = logger;
    unpacker_ = unpacker;
  }

  protected override async Task ExecuteAsync(CancellationToken token) {
    while (!token.IsCancellationRequested) {
      await unpacker_.ProcessBundles(token);
      await Task.Delay(2000, token);
    }
  }
}