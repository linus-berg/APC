using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace ACM.Rsync;

public class Collector : ICollector {
  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
  }
}