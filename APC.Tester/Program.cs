// See https://aka.ms/new-console-template for more information

using ACM.Git;
using ACM.Kernel;
using APC.Github;
using APC.Kernel;
using APC.Kernel.Models;
using APM.Github.Releases;
using Foundatio.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly;
using Polly.Registry;
using Polly.Retry;
using Polly.Timeout;
using StackExchange.Redis;

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

ServiceCollection services = new();

// Define a resilience pipeline with the name "my-pipeline"
services.AddResiliencePipeline<string, bool>("storage-pipeline", builder =>
                                               builder
                                                 .AddRetry(
                                                   new RetryStrategyOptions<
                                                     bool> {
                                                     Delay = TimeSpan
                                                       .FromSeconds(10),
                                                     MaxRetryAttempts = 10
                                                   }));
services.AddResiliencePipeline<string, bool>("git-timeout",
                                             builder => {
                                               builder.AddTimeout(
                                                 new
                                                   TimeoutStrategyOptions {
                                                     Timeout =
                                                       TimeSpan.FromMinutes(
                                                         10)
                                                   });
                                             });
services.AddResiliencePipeline<string, bool>(
  "git-retry", builder => {
    builder.AddRetry(new RetryStrategyOptions<bool> {
      Delay = TimeSpan.FromSeconds(5),
      MaxRetryAttempts = 5
    });
  });

// Build the service provider
IServiceProvider serviceProvider = services.BuildServiceProvider();

// Retrieve ResiliencePipelineProvider that caches and dynamically creates the resilience pipelines
ResiliencePipelineProvider<string> provider = serviceProvider
  .GetRequiredService<ResiliencePipelineProvider<string>>();

// Execute the pipeline
FileSystem fs = new(storage, provider);
Git git = new(fs, provider, new Logger<Git>(new NullLoggerFactory()));
IGithubReleases ghr = new GithubReleases(new GithubClient());


Artifact artifact = new() {
  id = "helm/helm"
};
IConnectionMultiplexer mx = ConnectionMultiplexer.Connect(
  Configuration.GetApcVar(ApcVariable.APC_REDIS_HOST));

artifact.config["files"] =
  @"^helm-v\d+.\d+.\d+-darwin-arm64.tar.gz.sha256sum.asc$";
//await ghr.ProcessArtifact(artifact);
await git.Mirror("git://github.com/linus-berg/ATM.Npm");
//await client.CopyToRegistry("docker://docker.io/registry:2");
Console.WriteLine("---");