using APM.Maven;
using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using MassTransit;

namespace Processor.Maven;

public class Consumer : IProcessor {
  private readonly IMaven maven_;

  public Consumer(IMaven maven) {
    maven_ = maven;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await maven_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}