using ACM.Http;
using APC.Kernel;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.RegisterCollector("http", new Collector());
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();