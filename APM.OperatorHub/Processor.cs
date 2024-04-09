using APC.Kernel;
using APC.Kernel.Extensions;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using APM.OperatorHub;
using MassTransit;

namespace APM.Nuget;

public class Processor : IProcessor {
  private readonly IOperatorHub operator_hub_;

  public Processor(IOperatorHub operator_hub) {
    operator_hub_ = operator_hub;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await operator_hub_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}