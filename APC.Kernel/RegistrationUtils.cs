using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace APC.Kernel;

public static class RegistrationUtils {
  public static IServiceCollection RegisterProcessor(this IServiceCollection sc, string name, IProcessor processor) {
    sc.AddMassTransit(mt => {
      mt.UsingRabbitMq((ctx, cfg) => {
        SetupRabbitMq(cfg);
        cfg.ReceiveEndpoint($"apm-{name}", e => {
          e.UseMessageRetry(r => r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(4),
            TimeSpan.FromMinutes(15)));
          e.Instance(processor);
        });
        cfg.ConfigureEndpoints(ctx);
      });
    });
    return sc;
  }

  public static IServiceCollection RegisterCollector(this IServiceCollection sc, string name, ICollector collector) {
    sc.AddMassTransit(mt => {
      mt.UsingRabbitMq((ctx, cfg) => {
        SetupRabbitMq(cfg);
        cfg.ReceiveEndpoint($"acm-{name}", e => {
          e.UseMessageRetry(
            r => r.Intervals(TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(4), TimeSpan.FromMinutes(15)));
          e.Instance(collector);
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