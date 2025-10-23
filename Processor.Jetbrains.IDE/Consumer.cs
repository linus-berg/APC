using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using MassTransit;

namespace Processor.Jetbrains.IDE;

public class Consumer : IProcessor {
  private readonly IJetbrains jetbrains_;

  public Consumer(IJetbrains jetbrains) {
    jetbrains_ = jetbrains;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await jetbrains_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}