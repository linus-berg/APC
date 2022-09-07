using APC.Kernel;
using APM.Nuget;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.RegisterProcessor("nuget", new Processor());
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();