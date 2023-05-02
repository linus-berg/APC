// See https://aka.ms/new-console-template for more information

using APM.Maven;
using MavenNet.Models;

string group = "org/eclipse/jetty/aggregate";

string id = "jetty-all";
string v = "2.0.0";

IMaven mvn = new Maven();
Metadata metadata = await mvn.GetMetadata(group, id);
Project p = await mvn.GetPom(group, id, v);

string lib = mvn.GetLibraryPath(group, id, v, p.Packaging);
string doc = mvn.GetDocsPath(group, id, v, "javadoc", "jar");
string src = mvn.GetSrcPath(group, id, v, "sources", "jar");
string pom = mvn.GetPomPath(group, id, v);

Console.WriteLine(lib);
Console.WriteLine(doc);
Console.WriteLine(src);
Console.WriteLine(pom);
Console.WriteLine("---");