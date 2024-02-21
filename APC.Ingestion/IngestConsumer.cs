using APC.Kernel.Messages;
using APC.Services;
using MassTransit;

namespace APC.Ingestion;

public class IngestConsumer : IConsumer<ArtifactIngestRequest> {
  private readonly IArtifactService aps_;
  private readonly IBus bus_;
  private readonly IApcCache cache_;
  private readonly ILogger<IngestConsumer> logger_;

  public IngestConsumer(ILogger<IngestConsumer> logger, IBus bus,
                        IApcCache cache,
                        IArtifactService aps) {
    logger_ = logger;
    bus_ = bus;
    cache_ = cache;
    aps_ = aps;
  }

  public async Task Consume(ConsumeContext<ArtifactIngestRequest> context) {
    /* Run as init */
    ArtifactIngestRequest request = context.Message;
    await aps_.Process(request.artifact);
  }
}