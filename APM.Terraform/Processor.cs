using APC.Kernel;
using APC.Kernel.Extensions;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Terraform;

public class Processor : IProcessor {
  private readonly ITerraform terraform_;

  public Processor(ITerraform terraform) {
    terraform_ = terraform;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await terraform_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}