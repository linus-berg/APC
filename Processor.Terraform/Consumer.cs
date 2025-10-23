using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using MassTransit;

namespace Processor.Terraform;

public class Consumer : IProcessor {
  private readonly ITerraform terraform_;

  public Consumer(ITerraform terraform) {
    terraform_ = terraform;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await terraform_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}