using ACM.Router;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;

ModuleRegistration registration = new(ModuleType.ACM, typeof(Router));
registration.AddEndpoint("router");

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.Register(registration);
                   services.AddTelemetry(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();