using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Registrations;
using APM.Jetbrains;

ModuleRegistration registration = new(ModuleType.APM, typeof(Processor));
registration.AddEndpoint("jetbrains");

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<IJetbrains, Jetbrains>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();