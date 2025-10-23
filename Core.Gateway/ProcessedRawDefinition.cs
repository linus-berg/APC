using Core.Kernel;
using MassTransit;

namespace Core.Gateway;

/* Endpoint for messages not using the standard MT format. */
public class ProcessedRawDefinition : ConsumerDefinition<ProcessedRawConsumer> {
  public ProcessedRawDefinition() {
    EndpointName = Endpoints.S_GATEWAY_INGEST_PROCESSED_RAW.ToString()
                            .Replace("queue:", "");
    ConcurrentMessageLimit = 10;
  }

  protected override void ConfigureConsumer(
    IReceiveEndpointConfigurator endpoint_configurator,
    IConsumerConfigurator<ProcessedRawConsumer> consumer_configurator) {
    // configure message retry with millisecond intervals
    endpoint_configurator.UseMessageRetry(
      r =>
        r.Intervals(100, 200, 500, 800, 1000)
    );
    // use the outbox to prevent duplicate events from being published
    endpoint_configurator.UseInMemoryOutbox();
    endpoint_configurator.UseRawJsonDeserializer(isDefault: true);
  }
}