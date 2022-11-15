// See https://aka.ms/new-console-template for more information

using APC.Services.Models;
using APM.Helm;
using ATM.Rancher;

Console.WriteLine("Hello, World!");
RancherProcessor processor = new("rancher/rancher", "rancher-images.txt");
await processor.CheckReleases();
Helm helm = new();

Artifact artifact = await helm.ProcessArtifact("rancher-stable/rancher");


Console.WriteLine(artifact.name);