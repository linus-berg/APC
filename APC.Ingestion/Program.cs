using APC.Infrastructure;
using APC.Infrastructure.Services;
using APC.Ingestion;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using APC.Services;
using MassTransit;
using StackExchange.Redis;

ModuleRegistration registration =
  new ModuleRegistration(ModuleType.APC, typeof(IHost));
IHost host = Host.CreateDefaultBuilder(args)
                 .AddLogging(registration)
                 .ConfigureServices(services => {
                   services.AddTelemetry(registration);
                   services.AddMassTransit(b => {
                     b.AddConsumer<ProcessedConsumer>(
                       typeof(ProcessedDefinition));
                     b.AddConsumer<ProcessedRawConsumer>(
                       typeof(ProcessedRawDefinition));
                     b.AddConsumer<IngestConsumer>(typeof(IngestDefinition));

                     b.UsingRabbitMq((ctx, cfg) => {
                       cfg.SetupRabbitMq();
                       cfg.ConfigureEndpoints(ctx);
                     });
                   });

                   services.AddSingleton<IConnectionMultiplexer>(
                     ConnectionMultiplexer.Connect(
                       Configuration.GetApcVar(ApcVariable.APC_REDIS_HOST)));
                   services.AddScoped<IApcDatabase, MongoDatabase>();
                   services.AddSingleton<IApcCache, ApcCache>();
                   services.AddScoped<IArtifactService, ArtifactService>();
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();