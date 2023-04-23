using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace APC.Kernel;

public static class RegistrationUtils {
  public static IServiceCollection RegisterProcessor<T, X>(
    this IServiceCollection sc) where T : class, IProcessor
                                where X : class, IConsumerDefinition {
    sc.AddMassTransit(mt => {
      mt.AddConsumer<T>(typeof(X));
      mt.UsingRabbitMq((ctx, cfg) => {
        cfg.SetupRabbitMq();
        cfg.ConfigureEndpoints(ctx);
      });
    });

    return sc;
  }

  public static IServiceCollection RegisterCollector(
    this IServiceCollection sc, IEnumerable<string> names,
    ICollector collector, int concurrency = 10) {
    sc.AddMassTransit(mt => {
      mt.UsingRabbitMq((ctx, cfg) => {
        cfg.SetupRabbitMq();
        foreach (string name in names) {
          cfg.ReceiveEndpoint($"acm-{name}", e => {
            e.ConfigureRetrying();
            e.ConcurrentMessageLimit = concurrency;
            e.Instance(collector);
          });
        }

        cfg.ConfigureEndpoints(ctx);
      });
    });
    return sc;
  }

  public static void ConfigureRetrying(
    this IRabbitMqReceiveEndpointConfigurator endpoint) {
    endpoint.UseDelayedRedelivery(r => {
      r.Intervals(TimeSpan.FromMinutes(5),
                  TimeSpan.FromMinutes(15),
                  TimeSpan.FromMinutes(30));
    });
    endpoint.UseMessageRetry(r => r.Immediate(5));
  }

  public static void SetupRabbitMq(this IRabbitMqBusFactoryConfigurator cfg) {
    cfg.Host(Configuration.GetApcVar(ApcVariable.APC_RABBIT_MQ_HOST), "/",
             h => {
               h.Username(
                 Configuration.GetApcVar(ApcVariable.APC_RABBIT_MQ_USER));
               h.Password(
                 Configuration.GetApcVar(ApcVariable.APC_RABBIT_MQ_PASS));
             });
  }
}