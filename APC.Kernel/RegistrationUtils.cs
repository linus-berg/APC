using APC.Kernel.Messages;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace APC.Kernel;

public static class RegistrationUtils {
  public static IServiceCollection RegisterIngestion(this IServiceCollection sc, IConsumer<ArtifactProcessedRequest> consumer) {
    sc.AddMassTransit(mt => {
      mt.UsingRabbitMq((ctx, cfg) => {
        SetupRabbitMq(cfg);
        cfg.ReceiveEndpoint(Endpoints.APC_INGEST.ToString().Replace("queue:", ""), e => { e.Instance(consumer); });
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
        cfg.ConfigureEndpoints(ctx);
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