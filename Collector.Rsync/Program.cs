using Collector.Rsync;
using Core.Kernel;
using Core.Kernel.Constants;
using Core.Kernel.Extensions;
using Core.Kernel.Registrations;

ModuleRegistration registration = new(ModuleType.COLLECTOR, typeof(Consumer));
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