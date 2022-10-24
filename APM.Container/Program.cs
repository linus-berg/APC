using APC.Kernel;
using APM.Container;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.RegisterProcessor("container", new Processor());
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();