using ACM.Git;
using ACM.Kernel;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using Polly;
using Polly.Timeout;

ModuleRegistration registration = new(ModuleType.ACM, typeof(Collector));
registration.AddEndpoint("git");

IHost host = Host.CreateDefaultBuilder(args)
                 .AddLogging(registration)
                 .ConfigureServices(
                   services => {
                     services.AddTelemetry(registration);
                     services.AddStorage();
                     services.AddResiliencePipeline<string, bool>(
                       "git-timeout",
                       builder => {
                         builder.AddTimeout(
                           new
                             TimeoutStrategyOptions {
                               Timeout =
                                 TimeSpan.FromMinutes(120)
                             }
                         );
                       }
                     );
                     services.AddSingleton<FileSystem>();
                     services.AddSingleton<Git>();
                     services.Register(registration);
                     services.AddHostedService<Worker>();
                   }
                 )
                 .Build();

await host.RunAsync();