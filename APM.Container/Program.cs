using APC.Kernel;
using APC.Skopeo;
using APM.Container;

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<SkopeoClient>();
                   services.RegisterProcessor<Processor, ProcessorDefinition>();
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();