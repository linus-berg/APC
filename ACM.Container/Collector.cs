using ACM.Kernel;
using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services.Models;
using APC.Skopeo;
using MassTransit;

namespace ACM.Container; 

public class Collector : ICollector {
  private readonly FileSystem fs_ = new();
  private readonly SkopeoClient skopeo_ = new SkopeoClient();

  public Collector() {
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    await skopeo_.CopyToOci(context.Message.location, fs_.GetModuleDir(context.Message.module));    
  }
}