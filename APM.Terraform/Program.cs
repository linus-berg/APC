using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using APM.Terraform;

ModuleRegistration registration = new(ModuleType.APM, typeof(Processor));
registration.AddEndpoint("terraform");

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddTelemetry(registration);
                   services.AddSingleton<ITerraform, Terraform>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();