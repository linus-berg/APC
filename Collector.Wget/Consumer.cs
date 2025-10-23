using Core.Kernel;
using Core.Kernel.Messages;
using MassTransit;

namespace ACM.Wget;

public class Consumer : ICollector {
  private readonly Wget wget_;

  public Consumer(Wget wget) {
    wget_ = wget;
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
    await wget_.Mirror(location);
  }
}