using APC.Infrastructure;
using APC.Ingestion;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.AddMassTransit(b => {
      b.AddConsumer<Engine>(typeof(EngineDefinition));
      b.UsingRabbitMq((ctx, cfg) => {
        cfg.Host("localhost", "/", h => {
          h.Username("guest");
          h.Password("guest");
        });
        cfg.ConfigureEndpoints(ctx);
      });
    });
    services.AddScoped<Database>();
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();