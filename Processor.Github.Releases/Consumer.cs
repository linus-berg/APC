using APM.Github.Releases;
using Core.Kernel;
using Core.Kernel.Extensions;
using Core.Kernel.Messages;
using Core.Kernel.Models;
using MassTransit;

namespace Processor.Github.Releases;

public class Consumer : IProcessor {
  private readonly IGithubReleases gh_;

  public Consumer(IGithubReleases gh) {
    gh_ = gh;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await gh_.ProcessArtifact(artifact);
    await context.ProcessorReply(artifact);
  }
}