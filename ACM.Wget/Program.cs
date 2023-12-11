using ACM.Kernel;
using ACM.Wget;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Registrations;
using Foundatio.Storage;

ModuleRegistration registration = new(ModuleType.ACM, typeof(Collector));
registration.AddEndpoint("wget");
IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddSingleton<IFileStorage>(new FolderFileStorage(
                       b => {
                         b.Folder(Configuration.GetApcVar(ApcVariable.APC_ACM_DIR));
                         return b;
                       }));
                   services.AddSingleton<FileSystem>();
                   services.AddSingleton<Wget>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();