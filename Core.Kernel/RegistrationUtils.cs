using Core.Kernel.Exceptions;
using Core.Kernel.Registrations;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Kernel;

public static class RegistrationUtils {
  public static IServiceCollection Register(this IServiceCollection sc,
                                            ModuleRegistration registration) {
    sc.AddMassTransit(
      mt => {
        mt.AddConsumer(registration.consumer);
        mt.UsingRabbitMq(
          (ctx, cfg) => {
            /* Absurdly high timeout */
            cfg.UseTimeout(t => t.Timeout = TimeSpan.FromMinutes(180));
            foreach (Endpoint endpoint in registration.endpoints) {
              cfg.ReceiveEndpoint(
                endpoint.name,
                c => {
                  c.ConfigureRetrying();
                  c.ConcurrentMessageLimit = endpoint.concurrency;

                  // use the outbox to prevent duplicate events from being published
                  c.UseInMemoryOutbox(ctx);
                  /* Absurdly high timeout */
                  c.UseTimeout(x => x.Timeout = TimeSpan.FromMinutes(180));
                  c.ConfigureConsumer(ctx, registration.consumer);
                }
              );
            }

            cfg.SetupRabbitMq();
            cfg.ConfigureEndpoints(ctx);
          }
        );
      }
    );
    return sc;
  }

  private static void ConfigureRetrying(
    this IRabbitMqReceiveEndpointConfigurator endpoint) {
    endpoint.UseDelayedRedelivery(
      r => {
        r.Handle<ArtifactTimeoutException>();
        r.Ignore<ArtifactMetadataException>();
        r.Intervals(
          TimeSpan.FromMinutes(5),
          TimeSpan.FromMinutes(15),
          TimeSpan.FromMinutes(30)
        );
      }
    );
    endpoint.UseMessageRetry(
      r => {
        r.Handle<ArtifactTimeoutException>();
        r.Ignore<ArtifactMetadataException>();
        r.Immediate(5);
      }
    );
  }

  public static void SetupRabbitMq(this IRabbitMqBusFactoryConfigurator cfg) {
    cfg.Host(
      Configuration.GetApcVar(CoreVariables.APC_RABBIT_MQ_HOST),
      "/",
      h => {
        h.Username(Configuration.GetApcVar(CoreVariables.APC_RABBIT_MQ_USER));
        h.Password(Configuration.GetApcVar(CoreVariables.APC_RABBIT_MQ_PASS));
      }
    );
  }
}