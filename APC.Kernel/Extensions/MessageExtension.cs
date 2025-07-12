using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APC.Kernel.Extensions;

public static class MessageExtension {
  public static async Task Collect(this ConsumeContext ctx, string location,
                                   string processor) {
    ArtifactCollectRequest request = new() {
      location = location,
      module = processor
    };
    await ctx.Send(
      new Uri($"queue:{request.GetCollectorModule()}"),
      request
    );
  }

  public static async Task ProcessorReply(
    this ConsumeContext<ArtifactProcessRequest> context,
    Artifact artifact) {
    await context.Send(
      Endpoints.S_APC_INGEST_PROCESSED,
      new ArtifactProcessedRequest {
        context = context.Message.ctx,
        artifact = artifact
      }
    );
  }
}