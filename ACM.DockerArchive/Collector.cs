using APC.Kernel;
using APC.Kernel.Messages;
using APC.Skopeo;
using MassTransit;

namespace ACM.DockerArchive;

public class Collector : ICollector {
  private readonly Docker docker_;
  public Collector(Docker docker) {
    docker_ = docker;
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    ArtifactCollectRequest request = context.Message;
    /* Collect if missing manifest or layers */
    await docker_.GetTarArchive(request.location);

  }
}