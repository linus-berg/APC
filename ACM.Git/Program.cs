using ACM.Git;
using APC.Kernel;

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.RegisterCollector(new List<string> {
                     "git"
                   }, new Collector());
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();