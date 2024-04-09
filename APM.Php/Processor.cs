using APC.Kernel;
using APC.Kernel.Extensions;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Php;

public class Processor : IProcessor {
  private readonly IPhp php_;

  public Processor(IPhp php) {
    php_ = php;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await php_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}