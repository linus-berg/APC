// See https://aka.ms/new-console-template for more information

using ACM.Git;
using ACM.Kernel;
using APC.Github;
using APC.Infrastructure;
using APC.Infrastructure.Services;
using APC.Kernel;
using APC.Kernel.Models;
using APC.Services;
using APM.Github.Releases;
using Foundatio.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Polly;
using Polly.Registry;
using Polly.Retry;
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

FileSystem fs = new(storage);

var services = new ServiceCollection();

// Define a resilience pipeline with the name "my-pipeline"
services.AddResiliencePipeline("minio-retry", builder =>
                                 builder
                                   .AddRetry(new RetryStrategyOptions() {
                                     Delay = TimeSpan.FromSeconds(10),
                                     MaxRetryAttempts = 10,
                                   }));

// Build the service provider
IServiceProvider serviceProvider = services.BuildServiceProvider();

// Retrieve ResiliencePipelineProvider that caches and dynamically creates the resilience pipelines
ResiliencePipelineProvider<string> provider = serviceProvider.GetRequiredService<ResiliencePipelineProvider<string>>();

// Execute the pipeline
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