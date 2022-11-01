// See https://aka.ms/new-console-template for more information

using APC.Skopeo;
using ATM.RKE2;
using ATM.RKE2.Models;

Console.WriteLine("Hello, World!");
RancherProcessor processor = new RancherProcessor();
await processor.CheckReleases();