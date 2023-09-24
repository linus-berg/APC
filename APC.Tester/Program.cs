// See https://aka.ms/new-console-template for more information

using ACM.Http;
using ACM.Kernel;
using APC.Kernel.Models;
using APC.Skopeo;
using APM.Jetbrains.IDE;
using APM.Pypi;
using Foundatio;
using Foundatio.Storage;
using IJetbrains = APM.Jetbrains.IDE.IJetbrains;
//Artifact p = await pypi.ProcessArtifact(artifact);


SkopeoClient client = new SkopeoClient();


await client.CopyToRegistry("docker://docker.io/registry:2");
Console.WriteLine("---");