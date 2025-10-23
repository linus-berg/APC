using Core.Gateway;
using Core.Infrastructure;
using Core.Infrastructure.Services;
using Core.Kernel;
using Core.Kernel.Constants;
using Core.Kernel.Extensions;
using Core.Kernel.Registrations;
using Core.Services;
using MassTransit;
using StackExchange.Redis;

ModuleRegistration registration = new(ModuleType.CORE, typeof(IHost));
IHost host = Host.CreateDefaultBuilder(args)
                 .AddLogging(registration)
                 .ConfigureServices(
                   services => {
                     services.AddTelemetry(registration);
                     services.AddMassTransit(
                       b => {
                         b.AddConsumer<ProcessedConsumer>(
                           typeof(ProcessedDefinition)
                         );
                         b.AddConsumer<ProcessedRawConsumer>(
                           typeof(ProcessedRawDefinition)
                         );
                         b.AddConsumer<IngestConsumer>(
                           typeof(IngestDefinition)
                         );

                         b.UsingRabbitMq(
                           (ctx, cfg) => {
                             cfg.SetupRabbitMq();
                             cfg.ConfigureEndpoints(ctx);
                           }
                         );
                       }
                     );

                     services.AddSingleton<IConnectionMultiplexer>(
                       ConnectionMultiplexer.Connect(
                         Configuration.GetApcVar(CoreVariables.APC_REDIS_HOST)
                       )
                     );
                     services.AddScoped<IApcDatabase, MongoDatabase>();
                     services.AddSingleton<IApcCache, ApcCache>();
                     services.AddScoped<IArtifactService, ArtifactService>();
                     services.AddHostedService<Worker>();
                   }
                 )
                 .Build();

await host.RunAsync();