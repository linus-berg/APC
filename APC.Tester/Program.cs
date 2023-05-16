// See https://aka.ms/new-console-template for more information

using APC.Kernel.Models;
using APM.Maven;

Maven mvn = new();
string g = "org.elasticsearch";
string c = "elasticsearch";

Artifact p = await mvn.ProcessArtifact(new Artifact {
  id = c,
  config = new Dictionary<string, string> {
    {
      "group", g
    }
  }
});
Dictionary<string, List<string>> src =
  await mvn.SearchMaven("org.elasticsearch", "elasticsearch");

Console.WriteLine("---");