using APC.Kernel;
using Collector.Kernel.Storage.Minio;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Retry;

namespace Collector.Kernel;

public static class StorageExtensions {
  public static IServiceCollection AddStorage(
    this IServiceCollection services) {
    services.AddResiliencePipeline<string, bool>(
      "storage-pipeline",
      builder => {
        builder.AddRetry(
          new RetryStrategyOptions<bool> {
            Delay = TimeSpan.FromSeconds(5),
            MaxRetryAttempts = 5
          }
        );
      }
    );
    /* SETUP STORAGE */
    MinioConnectionBuilder connection = new();

    connection.region = Configuration.GetApcVar(ApcVariable.ACM_S3_REGION);
    connection.access_key =
      Configuration.GetApcVar(ApcVariable.ACM_S3_ACCESS_KEY);
    connection.secret_key =
      Configuration.GetApcVar(ApcVariable.ACM_S3_SECRET_KEY);
    connection.end_point = Configuration.GetApcVar(ApcVariable.ACM_S3_ENDPOINT);
    connection.bucket = Configuration.GetApcVar(ApcVariable.ACM_S3_BUCKET);

    MinioStorageOptions minio_options = new() {
      auto_create_bucket = true,
      connection_string = connection.ToString()
    };
    services.AddSingleton(minio_options);
    services.AddSingleton<MinioStorage>();
    return services;
  }
}