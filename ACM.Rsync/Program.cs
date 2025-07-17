using ACM.Rsync;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;

ModuleRegistration registration = new(ModuleType.ACM, typeof(Collector));
registration.AddEndpoint("rsync");
IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(
                   services => {
                     services.AddTelemetry(registration);
                     services.Register(registration);
                     services.AddSingleton<RSync>();
                     services.AddHostedService<Worker>();
                   }
                 )
                 .Build();

await host.RunAsync();