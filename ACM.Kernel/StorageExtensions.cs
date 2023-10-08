using APC.Kernel;
using Foundatio.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ACM.Kernel;

public static class StorageExtensions {
  public static IServiceCollection AddStorage(
    this IServiceCollection services) {
    /* SETUP STORAGE */
    MinioFileStorageConnectionStringBuilder connection = new();

    connection.Region = Configuration.GetApcVar(ApcVariable.ACM_S3_REGION);
    connection.AccessKey =
      Configuration.GetApcVar(ApcVariable.ACM_S3_ACCESS_KEY);
    connection.SecretKey =
      Configuration.GetApcVar(ApcVariable.ACM_S3_SECRET_KEY);
    connection.EndPoint = Configuration.GetApcVar(ApcVariable.ACM_S3_ENDPOINT);
    connection.Bucket = Configuration.GetApcVar(ApcVariable.ACM_S3_BUCKET);

    MinioFileStorageOptions minio_options = new() {
      AutoCreateBucket = true,
      ConnectionString = connection.ToString()
    };
    MinioFileStorage storage = new(minio_options);
    services.AddSingleton<IFileStorage>(storage);
    return services;
  }
}