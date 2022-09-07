using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace APC.Kernel;

public static class StartupUtils {
  public static IServiceCollection RegisterIngestion(this IServiceCollection sc) {
    sc.AddMassTransit(mt => {
      mt.UsingRabbitMq((ctx, cfg) => {
        SetupRabbitMq(cfg);
        cfg.ConfigureEndpoints(ctx);
      });
    });
    return sc;
  }

  public static IServiceCollection RegisterProcessor(this IServiceCollection sc, string name, IProcessor processor) {
    sc.AddMassTransit(mt => {
      mt.UsingRabbitMq((ctx, cfg) => {
        SetupRabbitMq(cfg);
        cfg.ReceiveEndpoint($"{name}-module", e => { e.Instance(processor); });
      });
    });
    return sc;
  }

  private static void SetupRabbitMq(IRabbitMqBusFactoryConfigurator cfg) {
    cfg.Host("localhost", "/", h => {
      h.Username("guest");
      h.Password("guest");
    });
  }
}