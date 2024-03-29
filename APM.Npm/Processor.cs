using APC.Kernel;
using APC.Kernel.Extensions;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Npm;

public class Processor : IProcessor {
  private readonly INpm npm_;

  public Processor(INpm npm) {
    npm_ = npm;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await npm_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}