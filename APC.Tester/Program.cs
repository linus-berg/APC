// See https://aka.ms/new-console-template for more information

using APC.Kernel.Models;
using APM.Jetbrains;

IJetbrains jetbrains = new Jetbrains();
Artifact artifact = new() {
  id = "164-ideavim",
  root = true
};
Artifact a2 = await jetbrains.ProcessArtifact(artifact);

Console.WriteLine("---");