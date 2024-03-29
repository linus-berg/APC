using APC.Kernel;
using APC.Kernel.Messages;
using APC.Skopeo;
using MassTransit;

namespace ACM.Container;

public class Collector : ICollector {
  private readonly SkopeoClient skopeo_;

  public Collector(SkopeoClient skopeo) {
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