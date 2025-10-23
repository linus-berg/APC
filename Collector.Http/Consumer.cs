using Collector.Kernel;
using Core.Kernel;
using Core.Kernel.Messages;
using MassTransit;

namespace Collector.Http;

public class Consumer : ICollector {
  private readonly bool delta_;
  private readonly bool forward_;
  private readonly FileSystem fs_;

  public Consumer(FileSystem fs) {
    fs_ = fs;
    delta_ = Configuration.GetBackpackVariable(CoreVariables.BP_COLLECTOR_HTTP_DELTA) == "true";
    forward_ = Configuration.GetBackpackVariable(CoreVariables.BP_COLLECTOR_HTTP_MODE) == "forward";
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
    string fp = fs_.GetArtifactPath(module, location);
    bool exists = await fs_.Exists(fp);
    if (!exists || context.Message.force) {
      RemoteFile rf = new(location, fs_);
      if (await rf.Get(fp)) {
        if (delta_) {
          await fs_.CreateDeltaLink(module, location);
        }
      }
    }
  }
}