using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using APM.Jetbrains.IDE;

ModuleRegistration registration = new(ModuleType.APM, typeof(Processor));
registration.AddEndpoint("jetbrains-ide");
IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddTelemetry(registration);
                   services.AddSingleton<IJetbrains, Jetbrains>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();