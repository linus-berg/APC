using Core.Kernel;
using MassTransit;

namespace Core.Gateway;

public class IngestDefinition : ConsumerDefinition<IngestConsumer> {
  public IngestDefinition() {
    EndpointName = Endpoints.S_APC_INGEST_UNPROCESSED.ToString()
                            .Replace("queue:", "");
    ConcurrentMessageLimit = 10;
  }

  protected override void ConfigureConsumer(
    IReceiveEndpointConfigurator endpoint_configurator,
    IConsumerConfigurator<IngestConsumer> consumer_configurator) {
    // configure message retry with millisecond intervals
    endpoint_configurator.UseMessageRetry(
      r =>
        r.Intervals(100, 200, 500, 800, 1000)
    );
    // use the outbox to prevent duplicate events from being published
    endpoint_configurator.UseInMemoryOutbox();
  }
}