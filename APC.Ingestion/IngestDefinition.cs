using APC.Kernel;
using MassTransit;

namespace APC.Ingestion;

public class IngestDefinition : ConsumerDefinition<IngestConsumer> {
  public IngestDefinition() {
    EndpointName = Endpoints.APC_INGEST_UNPROCESSED.ToString().Replace("queue:", "");
    ConcurrentMessageLimit = 10;
  }
  protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
    IConsumerConfigurator<IngestConsumer> consumerConfigurator) {
    // configure message retry with millisecond intervals
    endpointConfigurator.UseMessageRetry(r => r.Intervals(100, 200, 500, 800, 1000));
    // use the outbox to prevent duplicate events from being published
    endpointConfigurator.UseInMemoryOutbox();
  }
}