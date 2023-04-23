using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace ACM.Git;

public class Collector : ICollector {
  private readonly Git git_;

  public Collector(Git git) {
    git_ = git;
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
    await git_.Mirror(location);
  }
}