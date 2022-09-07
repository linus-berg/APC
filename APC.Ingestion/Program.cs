using APC.Ingestion;
using APC.Kernel;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.RegisterIngestion();
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();