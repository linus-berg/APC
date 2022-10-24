using APC.Kernel.Exceptions;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace APC.Kernel;

public static class RegistrationUtils {
  public static IServiceCollection RegisterProcessor(this IServiceCollection sc, string name, IProcessor processor) {
    sc.AddMassTransit(mt => {
      mt.UsingRabbitMq((ctx, cfg) => {
        SetupRabbitMq(cfg);
        cfg.ReceiveEndpoint($"apm-{name}", e => {
          e.UseDelayedRedelivery(r => { 
            r.Handle<ArtifactTimeoutException>();
            r.Ignore<ArtifactMetadataException>();
            r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30));
          });
          e.UseMessageRetry(r => {
            r.Handle<ArtifactTimeoutException>();
            r.Ignore<ArtifactMetadataException>();
            r.Immediate(5);
          });
          e.Instance(processor);
        });
        cfg.ConfigureEndpoints(ctx);
      });
    });
    return sc;
  }

  public static IServiceCollection RegisterCollector(this IServiceCollection sc, IEnumerable<string> names, ICollector collector, int concurrency = 10) {
    sc.AddMassTransit(mt => {
      mt.UsingRabbitMq((ctx, cfg) => {
        SetupRabbitMq(cfg);
        foreach (string name in names) {
          cfg.ReceiveEndpoint($"acm-{name}", e => {
            e.UseDelayedRedelivery(r => {
              r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30));
            });
            e.UseMessageRetry(r => r.Immediate(5));
            e.ConcurrentMessageLimit = concurrency;
            e.Instance(collector);
          });
        }
        cfg.ConfigureEndpoints(ctx);
      });
    });
    return sc;
  }

  public static IServiceCollection RegisterRouter(this IServiceCollection sc, IRouter router) {
    sc.AddMassTransit(mt => {
      mt.UsingRabbitMq((ctx, cfg) => {
        SetupRabbitMq(cfg);
        cfg.ReceiveEndpoint("acm-router", e => {
          e.UseDelayedRedelivery(r => {
            r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30));
          });
          e.UseMessageRetry(r => r.Immediate(5));
          e.Instance(router);
        });
        cfg.ConfigureEndpoints(ctx);
      });
    });
    return sc;
  }

  private static void SetupRabbitMq(IRabbitMqBusFactoryConfigurator cfg) {
    cfg.Host(Configuration.GetAPCVar(ApcVariable.APC_RABBIT_MQ_HOST), "/", h => {
      h.Username(Configuration.GetAPCVar(ApcVariable.APC_RABBIT_MQ_USER));
      h.Password(Configuration.GetAPCVar(ApcVariable.APC_RABBIT_MQ_PASS));
    });
  }
}
