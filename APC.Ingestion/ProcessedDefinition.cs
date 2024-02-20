using APC.Kernel;
using MassTransit;

namespace APC.Ingestion;

public class ProcessedDefinition : ConsumerDefinition<ProcessedConsumer> {
  public ProcessedDefinition() {
    EndpointName = Endpoints.S_APC_INGEST_PROCESSED.ToString()
                            .Replace("queue:", "");
    ConcurrentMessageLimit = 10;
  }

  protected override void ConfigureConsumer(
    IReceiveEndpointConfigurator endpoint_configurator,
    IConsumerConfigurator<ProcessedConsumer> consumer_configurator) {
    // configure message retry with millisecond intervals
    endpoint_configurator.UseMessageRetry(r =>
                                            r.Intervals(
                                              100, 200, 500, 800, 1000));
    // use the outbox to prevent duplicate events from being published
    endpoint_configurator.UseInMemoryOutbox();
  }
}