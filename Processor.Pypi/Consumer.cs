using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using MassTransit;

namespace Processor.Pypi;

public class Consumer : IProcessor {
  private readonly IPypi pypi_;

  public Consumer(IPypi pypi) {
    pypi_ = pypi;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await pypi_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}