using APC.Kernel;
using APC.Kernel.Extensions;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Rancher;

public class Processor : IProcessor {
  private readonly IRancher rancher_;

  public Processor(IRancher rancher) {
    rancher_ = rancher;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await rancher_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}