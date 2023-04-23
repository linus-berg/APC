using APC.Kernel;
using APM.Helm;

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<Helm>();
                   services.RegisterProcessor<Processor, ProcessorDefinition>();
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();