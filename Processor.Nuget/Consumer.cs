using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using MassTransit;

namespace Processor.Nuget;

public class Consumer : IProcessor {
  private readonly INuget nuget_;

  public Consumer(INuget nuget) {
    nuget_ = nuget;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await nuget_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}