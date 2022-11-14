// See https://aka.ms/new-console-template for more information

using ATM.Rancher;

Console.WriteLine("Hello, World!");
RancherProcessor processor = new("rancher/rancher", "rancher-images.txt");
await processor.CheckReleases();