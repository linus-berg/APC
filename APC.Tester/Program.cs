// See https://aka.ms/new-console-template for more information

using APC.Kernel.Models;
using APM.Pypi;

Pypi mvn = new();
string g = "org.elasticsearch";
string c = "pandas";

Artifact p = await mvn.ProcessArtifact(new Artifact {
  id = c,
});
Console.WriteLine("---");