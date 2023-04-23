using APC.Kernel;
using APM.Nuget;

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<INuget, Nuget>();
                   services.RegisterProcessor<Processor, ProcessorDefinition>();
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();