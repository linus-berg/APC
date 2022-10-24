using ACM.Router;
using APC.Kernel;

IHost host = Host.CreateDefaultBuilder(args)
  .ConfigureServices(services => {
    services.RegisterRouter(new Router());
    services.AddHostedService<Worker>();
  })
  .Build();

await host.RunAsync();