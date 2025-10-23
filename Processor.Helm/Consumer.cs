using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using MassTransit;

namespace Processor.Helm;

public class Consumer : IProcessor {
  private readonly APM.Helm.Helm helm_;

  public Consumer(APM.Helm.Helm helm) {
    helm_ = helm;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await helm_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}