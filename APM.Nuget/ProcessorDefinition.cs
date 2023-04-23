using APC.Kernel.Exceptions;
using MassTransit;

namespace APM.Nuget;

public class ProcessorDefinition : ConsumerDefinition<Processor> {
  public ProcessorDefinition() {
    EndpointName = "apm-nuget";
    ConcurrentMessageLimit = 10;
  }

  protected override void ConfigureConsumer(
    IReceiveEndpointConfigurator endpoint_configurator,
    IConsumerConfigurator<Processor> consumer_configurator) {
    endpoint_configurator.UseDelayedRedelivery(r => {
      r.Handle<ArtifactTimeoutException>();
      r.Ignore<ArtifactMetadataException>();
      r.Intervals(TimeSpan.FromMinutes(5),
                  TimeSpan.FromMinutes(15),
                  TimeSpan.FromMinutes(30));
    });
    endpoint_configurator.UseMessageRetry(r => {
      r.Handle<ArtifactTimeoutException>();
      r.Ignore<ArtifactMetadataException>();
      r.Immediate(5);
    });

    // use the outbox to prevent duplicate events from being published
    endpoint_configurator.UseInMemoryOutbox();
  }
}