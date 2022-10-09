using ACM.Kernel;
using APC.Infrastructure.Models;
using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace ACM.Http;

public class Collector : ICollector {
  private readonly FileSystem fs_ = new();

  public Collector() {
    fs_.CreateDailyDeposit();
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    Artifact artifact = context.Message.Artifact;
    foreach (KeyValuePair<string, ArtifactVersion> kv in artifact.versions) {
      string fp = fs_.GetArtifactPath(artifact.module, kv.Value.location);
      if (fs_.Exists(fp) || fs_.Exists(fp + ".tmp")) continue;
      RemoteFile rf = new(kv.Value.location);
      await rf.Get(fp);
      fs_.CreateDailyLink(artifact.module, kv.Value.location);
    }
  }
}