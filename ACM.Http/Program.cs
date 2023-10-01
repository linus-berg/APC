using ACM.Http;
using ACM.Kernel;
using APC.Kernel;
using APC.Kernel.Constants;
using APC.Kernel.Extensions;
using APC.Kernel.Registrations;
using Foundatio.Storage;

ModuleRegistration registration = new(ModuleType.ACM, typeof(Collector));
registration.AddEndpoint("http");
registration.AddEndpoint("https");

/* SETUP STORAGE */
MinioFileStorageConnectionStringBuilder connection = new();

connection.Region = Configuration.GetApcVar(ApcVariable.ACM_S3_REGION);
connection.AccessKey = Configuration.GetApcVar(ApcVariable.ACM_S3_ACCESS_KEY);
connection.SecretKey = Configuration.GetApcVar(ApcVariable.ACM_S3_SECRET_KEY);
connection.EndPoint = Configuration.GetApcVar(ApcVariable.ACM_S3_ENDPOINT);
connection.Bucket = Configuration.GetApcVar(ApcVariable.ACM_S3_BUCKET);

MinioFileStorageOptions minio_options = new() {
  AutoCreateBucket = true,
  ConnectionString = connection.ToString()
};
MinioFileStorage storage = new(minio_options);

IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureServices(services => {
                   services.AddTelemetry(registration);
                   services.AddSingleton<IFileStorage>(storage);
                   services.AddSingleton<FileSystem>();
                   services.Register(registration);
                   services.AddHostedService<Worker>();
                 })
                 .Build();

await host.RunAsync();