using ACM.Container;
using APC.Kernel;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.RegisterCollector(new List<string> {
      "docker",
      "oci"
    }, new Collector(), 1);
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();