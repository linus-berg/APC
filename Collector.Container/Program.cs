using ACM.Container;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using APC.Skopeo;

ModuleRegistration registration = new(ModuleType.ACM, typeof(Consumer));
registration.AddEndpoint("docker", 5);
registration.AddEndpoint("oci", 5);

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(
                   services => {
                     services.AddTelemetry(registration);
                     services.AddSingleton<SkopeoClient>();
                     services.Register(registration);
                     services.AddHostedService<Worker>();
                   }
                 )
                 .Build();

await host.RunAsync();