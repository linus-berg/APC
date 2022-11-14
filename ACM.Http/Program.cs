using ACM.Http;
using APC.Kernel;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.RegisterCollector(new List<string> { "http", "https" }, new Collector());
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();