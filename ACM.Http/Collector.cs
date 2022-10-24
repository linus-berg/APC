using ACM.Kernel;
using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services.Models;
using MassTransit;

namespace ACM.Http;

public class Collector : ICollector {
  private readonly FileSystem fs_ = new();

  public Collector() {
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
    string fp = fs_.GetArtifactPath(module, location);
    if (!fs_.Exists(fp) && !fs_.Exists(fp + ".tmp")) {
      RemoteFile rf = new(location);
      await rf.Get(fp);
      fs_.CreateDailyLink(module, location);
    }
  }
}
