using System;
using System.Threading.Tasks;
using APC.Kernel.Messages;
using MassTransit;

namespace APC.Kernel.Extensions;

public static class MessageExtension {
  public static async Task Collect(this ConsumeContext ctx, string location,
                                   string processor) {
    ArtifactCollectRequest request = new() {
      location = location,
      module = processor
    };
    await ctx.Send(new Uri($"queue:{request.GetCollectorModule()}"),
                   request);
  }
}