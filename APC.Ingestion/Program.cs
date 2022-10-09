using APC.Infrastructure;
using APC.Ingestion;
using MassTransit;
using StackExchange.Redis;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.AddMassTransit(b => {
      b.AddConsumer<Engine>(typeof(ProcessedDefinition));
      b.AddConsumer<IngestConsumer>(typeof(IngestDefinition));
      
      b.UsingRabbitMq((ctx, cfg) => {
        cfg.Host("localhost", "/", h => {
          h.Username("guest");
          h.Password("guest");
        });
        cfg.ConfigureEndpoints(ctx);
      });
    });
    
    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));
    services.AddScoped<Database>();
    services.AddSingleton<RedisCache>();
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();