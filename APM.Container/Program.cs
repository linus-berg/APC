using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Registrations;
using APC.Skopeo;
using APM.Container;

ModuleRegistration registration = new(ModuleType.APM, typeof(Processor));
registration.AddEndpoint("container");

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<SkopeoClient>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();