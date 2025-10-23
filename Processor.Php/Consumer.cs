using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using MassTransit;

namespace Processor.Php;

public class Consumer : IProcessor {
  private readonly IPhp php_;

  public Consumer(IPhp php) {
    php_ = php;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await php_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}