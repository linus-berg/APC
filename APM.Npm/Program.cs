using APC.Kernel;
using APM.Npm;

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<INpm, Npm>();
                   services.RegisterProcessor<Processor, ProcessorDefinition>();
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();