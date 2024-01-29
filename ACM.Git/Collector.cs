using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace ACM.Git;

public class Collector : ICollector {
  private readonly Git git_;
  private readonly ILogger<Collector> logger_;

  public Collector(Git git, ILogger<Collector> logger) {
    git_ = git;
    logger_ = logger;
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
    try {
      await git_.Mirror(location);
    } catch (Exception e) {
      logger_.LogError("{Location} failed with error {Error}", location,
                       e.ToString());
    }
  }
}