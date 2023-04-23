using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Registrations;
using APM.Helm;

ModuleRegistration registration = new(ModuleType.APM, typeof(Processor));
registration.AddEndpoint("helm");

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<Helm>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();