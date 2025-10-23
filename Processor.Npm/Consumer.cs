using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using MassTransit;

namespace Processor.Npm;

public class Consumer : IProcessor {
  private readonly INpm npm_;

  public Consumer(INpm npm) {
    npm_ = npm;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await npm_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}