using APM.Helm;
using Core.Kernel;
using Core.Kernel.Constants;
using Core.Kernel.Extensions;
using Core.Kernel.Registrations;
using Processor.Helm;

ModuleRegistration registration = new(ModuleType.PROCESSOR, typeof(Consumer));
registration.AddEndpoint("helm");

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(
                   services => {
                     services.AddTelemetry(registration);
                     services.AddSingleton<Helm>();
                     services.Register(registration);
                     services.AddHostedService<Worker>();
                   }
                 )
                 .Build();

await host.RunAsync();