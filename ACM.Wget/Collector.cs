using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace ACM.Wget;

public class Collector : ICollector {
  private readonly Wget wget_;

  public Collector(Wget wget) {
    wget_ = wget;
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
    await wget_.Mirror(location);
  }
}