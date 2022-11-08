// See https://aka.ms/new-console-template for more information

using APC.Skopeo;
using ATM.Rancher;
using ATM.Rancher.Models;

Console.WriteLine("Hello, World!");
RancherProcessor processor = new RancherProcessor("rancher/rancher", "rancher-images.txt");
await processor.CheckReleases();
