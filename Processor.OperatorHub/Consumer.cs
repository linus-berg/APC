using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using MassTransit;

namespace Processor.OperatorHub;

public class Consumer : IProcessor {
  private readonly IOperatorHub operator_hub_;

  public Consumer(IOperatorHub operator_hub) {
    operator_hub_ = operator_hub;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await operator_hub_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}