using Collector.DockerArchive;
using Collector.Kernel;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using APC.Skopeo;
using Polly;
using Polly.Retry;

ModuleRegistration registration = new(ModuleType.ACM, typeof(Consumer));
registration.AddEndpoint("docker-archive", 1);
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddTelemetry(registration);
builder.Services.AddStorage();
builder.Services.AddResiliencePipeline<string, bool>(
  "skopeo-retry",
  pipeline_builder => {
    pipeline_builder.AddRetry(
      new RetryStrategyOptions<bool> {
        MaxRetryAttempts = 20
      }
    );
  }
);
builder.Services.AddSingleton<SkopeoClient>();
builder.Services.AddSingleton<FileSystem>();
builder.Services.AddSingleton<Docker>();
builder.Services.Register(registration);
builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();