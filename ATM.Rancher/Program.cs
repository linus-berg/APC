using APC.Kernel;
using ATM.Rancher;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();