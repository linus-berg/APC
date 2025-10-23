using Core.Kernel;
using Core.Kernel.Messages;
using Library.Skopeo;
using MassTransit;

namespace Collector.Container;

public class Consumer : ICollector {
  private readonly SkopeoClient skopeo_;

  public Consumer(SkopeoClient skopeo) {
    skopeo_ = skopeo;
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    ArtifactCollectRequest request = context.Message;
    /* Collect if missing manifest or layers */

    SkopeoManifest? manifest = await skopeo_.ImageExists(request.location);
    if (manifest != null) {
      return;
    }

    await skopeo_.CopyToRegistry(request.location);
  }
}