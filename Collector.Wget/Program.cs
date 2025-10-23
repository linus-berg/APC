using Collector.Kernel;
using ACM.Wget;
using Core.Kernel;
using Core.Kernel.Constants;
using Core.Kernel.Registrations;
using Foundatio.Storage;

ModuleRegistration registration = new(ModuleType.COLLECTOR, typeof(Consumer));
registration.AddEndpoint("wget");
IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(
                   services => {
                     services.AddSingleton<IFileStorage>(
                       new FolderFileStorage(
                         b => {
                           b.Folder(
                             Configuration.GetApcVar(CoreVariables.APC_ACM_DIR)
                           );
                           return b;
                         }
                       )
                     );
                     services.AddSingleton<FileSystem>();
                     services.AddSingleton<Wget>();
                     services.Register(registration);
                     services.AddHostedService<Worker>();
                   }
                 )
                 .Build();

await host.RunAsync();