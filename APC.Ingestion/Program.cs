using APC.Infrastructure;
using APC.Ingestion;
using APC.Kernel;
using APC.Services;
using MassTransit;
using StackExchange.Redis;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.AddMassTransit(b => {
      b.AddConsumer<ProcessedConsumer>(typeof(ProcessedDefinition));
      b.AddConsumer<IngestConsumer>(typeof(IngestDefinition));

      b.UsingRabbitMq((ctx, cfg) => {
        cfg.Host(Configuration.GetAPCVar(ApcVariable.APC_RABBIT_MQ_HOST), "/", h => {
          h.Username(Configuration.GetAPCVar(ApcVariable.APC_RABBIT_MQ_USER));
          h.Password(Configuration.GetAPCVar(ApcVariable.APC_RABBIT_MQ_PASS));
        });
        cfg.ConfigureEndpoints(ctx);
      });
    });

    services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(Configuration.GetAPCVar(ApcVariable.APC_REDIS_HOST)));
    services.AddScoped<IApcDatabase, ApcDatabase>();
    services.AddSingleton<IApcCache, ApcCache>();
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();