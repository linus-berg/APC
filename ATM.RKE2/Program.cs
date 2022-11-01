using APC.Kernel;
using ATM.RKE2;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();