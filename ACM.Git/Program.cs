using ACM.Git;
using ACM.Kernel;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using Polly;
using Polly.Retry;
using Polly.Timeout;

ModuleRegistration registration = new(ModuleType.ACM, typeof(Collector));
registration.AddEndpoint("git");

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddTelemetry(registration);
                   services.AddStorage();
                   services.AddResiliencePipeline<string, bool>("git-timeout",
                     builder => {
                       builder.AddTimeout(new TimeoutStrategyOptions() {
                         Timeout = TimeSpan.FromMinutes(10)
                       });
                     });
                   services.AddResiliencePipeline<string, bool>("minio-retry", builder => {
                     builder.AddRetry<bool>(new RetryStrategyOptions<bool>() {
                       Delay = TimeSpan.FromSeconds(5),
                       MaxRetryAttempts = 5
                     });
                   });
                   services.AddSingleton<FileSystem>();
                   services.AddSingleton<Git>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();