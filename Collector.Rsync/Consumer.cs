using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace Collector.Rsync;

public class Consumer : ICollector {
  private readonly RSync rsync_;

  public Consumer(RSync rsync) {
    rsync_ = rsync;
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
    await rsync_.Mirror(location);
  }
}