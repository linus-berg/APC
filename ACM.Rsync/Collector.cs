using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace ACM.Rsync;

public class Collector : ICollector {
  private readonly RSync rsync_;

  public Collector(RSync rsync) {
    rsync_ = rsync;
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
    await rsync_.Mirror(location);
  }
}