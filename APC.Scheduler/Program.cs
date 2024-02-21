using System.Text.Json;
using APC.Infrastructure;
using APC.Infrastructure.Services;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using APC.Scheduler;
using APC.Services;
using MassTransit;
using Microsoft.IdentityModel.Protocols.Configuration;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using StackExchange.Redis;

ModuleRegistration registration = new(ModuleType.APC, typeof(IHost));

string? file = Environment.GetEnvironmentVariable("SCHEDULE_FILE");
if (file.IsNullOrEmpty()) {
  throw new InvalidConfigurationException("SCHEDULE_FILE is not defined.");
}

if (!File.Exists(file)) {
  throw new FileNotFoundException($"{file} not found.");
}

string schedule_str = await File.ReadAllTextAsync(file);
List<ScheduleOptions>? schedule_opts = JsonSerializer.Deserialize<List<ScheduleOptions>>(schedule_str);

if (schedule_opts == null) {
  throw new ArgumentNullException("Schedule could not be parsed");
}

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddTelemetry(registration);
                   services.AddHostedService<Worker>();
                   services.AddMassTransit(mt => {
                     mt.UsingRabbitMq((ctx, cfg) => {
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

                   services.AddQuartz(q => {
                     q.AddJob<TrackingJob>(
                       j => j.WithIdentity(TrackingJob.S_KEY));
                       foreach (ScheduleOptions opts in schedule_opts) {
                         q.AddTrigger(t => {
                             t.WithIdentity($"tracking-{opts.processor}", "apc");
                             t.ForJob(TrackingJob.S_KEY);
                             t.UsingJobData("processor", opts.processor);
                             t.WithCronSchedule(opts.schedule);
                         });
                       }
                   });

                   services.AddQuartzHostedService(q => {
                     q.WaitForJobsToComplete = true;
                   });
                 })
                 .Build();
await host.RunAsync();