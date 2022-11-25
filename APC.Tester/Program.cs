// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using APC.Services.Models;
using APM.Helm;
using ATM.Rancher;

Console.WriteLine("Hello, World!");
string str = "docker://docker.io/rancher-stable/rancher";
string str_b = "docker://rancher-stable/rancher";
Uri uri = new Uri(str);
Uri uri_b = new Uri(str_b);
bool is_uri = Uri.IsWellFormedUriString(str, UriKind.Relative);
bool is_match = Regex.IsMatch(str, @"\w+\.\w+\/");
Console.WriteLine(is_match);
Console.WriteLine(is_uri);
Console.WriteLine(Path.IsPathFullyQualified(str));
Console.WriteLine(uri.Host);
