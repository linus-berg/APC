// See https://aka.ms/new-console-template for more information

using ACM.Kernel;
using APC.Kernel.Models;
using APM.Jetbrains;
using APM.Maven;

IJetbrains jetbrains = new Jetbrains();
Artifact artifact = new Artifact() {
  id = "164-ideavim",
  root = true
};
Artifact a2 = await jetbrains.ProcessArtifact(artifact);

Console.WriteLine("---");