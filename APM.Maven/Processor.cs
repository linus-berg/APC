using APC.Kernel;
using APC.Kernel.Extensions;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Maven;

public class Processor : IProcessor {
  private readonly IMaven maven_;

  public Processor(IMaven maven) {
    maven_ = maven;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await maven_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}