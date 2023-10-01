using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using APM.Rancher;

ModuleRegistration registration = new(ModuleType.APM, typeof(Processor));
registration.AddEndpoint("rancher");
IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddTelemetry(registration);
                   services.AddSingleton<IGithubClient, GithubClient>();
                   services.AddSingleton<IRancher, Rancher>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();