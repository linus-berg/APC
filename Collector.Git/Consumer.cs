using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace Collector.Git;

public class Consumer : ICollector {
  private readonly Git git_;
  private readonly ILogger<Consumer> logger_;

  public Consumer(Git git, ILogger<Consumer> logger) {
    git_ = git;
    logger_ = logger;
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
    try {
      await git_.Mirror(location, context.CancellationToken);
    } catch (Exception e) {
      logger_.LogError(
        "{Location} failed with error {Error}",
        location,
        e.ToString()
      );
      throw;
    }
  }
}