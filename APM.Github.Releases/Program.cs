using APC.Github;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using APM.Github.Releases;

ModuleRegistration registration = new(ModuleType.APM, typeof(Processor));
registration.AddEndpoint("github-releases");
IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(
                   services => {
                     services.AddTelemetry(registration);
                     services.AddSingleton<IGithubClient, GithubClient>();
                     services.AddSingleton<IGithubReleases, GithubReleases>();
                     services.Register(registration);
                     services.AddHostedService<Worker>();
                   }
                 )
                 .Build();

await host.RunAsync();