using APC.Kernel;
using APM.Npm;

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.RegisterProcessor("npm", new Processor(new Npm()));
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();