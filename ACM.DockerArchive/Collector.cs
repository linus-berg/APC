using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;
using Polly;
using Polly.Registry;

namespace ACM.DockerArchive;

public class Collector : ICollector {
  private readonly Docker docker_;
  private readonly ResiliencePipeline<bool> pipeline_;

  public Collector(Docker docker, ResiliencePipelineProvider<string> polly) {
    pipeline_ = polly.GetPipeline<bool>("skopeo-retry");
    docker_ = docker;
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    ArtifactCollectRequest request = context.Message;
    /* Collect if missing manifest or layers */
    await pipeline_.ExecuteAsync(
      async (state, token) =>
        await docker_.GetTarArchive(state.location), request,
      context.CancellationToken);
  }
}