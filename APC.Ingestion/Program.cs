using APC.Infrastructure;
using APC.Ingestion;
using APC.Kernel;
using MassTransit;
using StackExchange.Redis;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.AddMassTransit(b => {
      b.AddConsumer<ProcessedConsumer>(typeof(ProcessedDefinition));
      b.AddConsumer<IngestConsumer>(typeof(IngestDefinition));

      b.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(Configuration.GetAPCVar(Configuration.APC_VAR.APC_RABBIT_MQ_HOST), "/", h => {
          h.Username(Configuration.GetAPCVar(Configuration.APC_VAR.APC_RABBIT_MQ_USER));
          h.Password(Configuration.GetAPCVar(Configuration.APC_VAR.APC_RABBIT_MQ_PASS));
        });
        cfg.ConfigureEndpoints(ctx);
      });
    });

    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetAPCVar(Configuration.APC_VAR.APC_REDIS_HOST)));
    services.AddScoped<Database>();
    services.AddSingleton<RedisCache>();
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();