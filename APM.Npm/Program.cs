using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Registrations;
using APM.Npm;

ModuleRegistration registration =
  new(ModuleType.APM, typeof(Processor));
registration.AddEndpoint("npm");

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<INpm, Npm>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();