using ACM.Router;
using APC.Kernel;
using MassTransit;

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddMassTransit(mt => {
                     mt.UsingRabbitMq((ctx, cfg) => {
                       cfg.SetupRabbitMq();
                       cfg.ReceiveEndpoint(
                         "acm-router", e => {
                           e.ConfigureRetrying();
                           e.Instance(new Router());
                         });
                       cfg.ConfigureEndpoints(ctx);
                     });
                   });
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();