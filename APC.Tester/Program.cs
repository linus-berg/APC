// See https://aka.ms/new-console-template for more information

using ACM.Git;
using ACM.Kernel;
using APC.Kernel.Models;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Timeout;

ServiceCollection services = new();
services.AddStorage();
// Define a resilience pipeline with the name "my-pipeline"
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

services.AddLogging();
services.AddSingleton<FileSystem>();
services.AddSingleton<Git>();
// Build the service provider
IServiceProvider sp = services.BuildServiceProvider();

// Execute the pipeline
Git git = sp.GetRequiredService<Git>();
Artifact artifact = new() {
  id = "helm/helm"
};
artifact.config["files"] =
  @"^helm-v\d+.\d+.\d+-darwin-arm64.tar.gz.sha256sum.asc$";
//await ghr.ProcessArtifact(artifact);
//await git.Mirror("git://github.com/linus-berg/ATM.Npm");
string ind = "/storage/artifacts/mirrors/git/input";
string path =
  $"{ind}/github.com/linus-berg/test@101001-201023.bundle";


Console.WriteLine(Path.GetDirectoryName(path));

Console.WriteLine(Path.GetDirectoryName(Path.GetRelativePath(ind, path)));
//await client.CopyToRegistry("docker://docker.io/registry:2");
Console.WriteLine("---");