// See https://aka.ms/new-console-template for more information

using ACM.Kernel;
using APM.Maven;
using MavenNet.Models;

string group = "org/eclipse/jetty/aggregate";

string id = "jetty-all";
string v = "2.0.0";

FileSystem fs = new FileSystem();

string p = fs.GetArtifactPath("npm", "https://registry.npmjs.org/react/bleh.tgz");

Console.WriteLine("---");