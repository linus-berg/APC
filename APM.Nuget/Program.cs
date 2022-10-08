using APC.Kernel;
using APM.Nuget;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.RegisterProcessor("nuget", new Processor(new Nuget()));
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();