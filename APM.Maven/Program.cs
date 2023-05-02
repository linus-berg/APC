using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Registrations;
using APM.Maven;

ModuleRegistration registration = new(ModuleType.APM, typeof(Processor));
registration.AddEndpoint("maven");

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<IMaven, Maven>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();