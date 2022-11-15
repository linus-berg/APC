using APC.Kernel;
using APM.Helm;

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.RegisterProcessor("helm",
                                              new Processor(new Helm()));
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();