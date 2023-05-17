using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Registrations;
using APM.Pypi;

ModuleRegistration registration = new(ModuleType.APM, typeof(Processor));
registration.AddEndpoint("pypi");

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<IPypi, Pypi>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();