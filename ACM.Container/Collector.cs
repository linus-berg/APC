using ACM.Kernel;
using APC.Kernel;
using APC.Kernel.Messages;
using APC.Skopeo;
using MassTransit;

namespace ACM.Container;

public class Collector : ICollector {
  private readonly FileSystem fs_ = new();
  private readonly SkopeoClient skopeo_ = new();

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    ArtifactCollectRequest request = context.Message;
    string wd = fs_.GetModuleDir(context.Message.module);

    SkopeoManifest? manifest = await skopeo_.ImageExists(request.location, wd);
    bool collect = false;
    
    /* If manifest does not exist on disk */
    if (manifest == null) {
      collect = true;
    } else {
      /* Verify all layers are present */
      collect = !manifest.VerifyLayers();
    }
    
    /* Collect if missing manifest or layers */
    if (collect) {
      await skopeo_.CopyToOci(request.location, wd);
    }
  }
}