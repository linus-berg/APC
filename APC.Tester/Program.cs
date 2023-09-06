// See https://aka.ms/new-console-template for more information

using APC.Kernel.Models;
using APC.Skopeo;
using APM.Jetbrains.IDE;
using APM.Pypi;
using IJetbrains = APM.Jetbrains.IDE.IJetbrains;

Pypi pypi = new();
SkopeoClient s = new();

Artifact artifact = new() {
  id = "IIU",
  processor = "jetbrains-ide"
};

IJetbrains jetbrains = new Jetbrains();

Artifact result = await jetbrains.ProcessArtifact(artifact);



//Artifact p = await pypi.ProcessArtifact(artifact);
Console.WriteLine("---");