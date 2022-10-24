// See https://aka.ms/new-console-template for more information

using APC.Skopeo;

Console.WriteLine("Hello, World!");
SkopeoClient client = new SkopeoClient();
SkopeoListTagsOutput output = await client.GetTags("docker://docker.io/nginx");

foreach (string tag in output.Tags) {
  Console.WriteLine(tag);
}

await client.CopyToOci(
  "docker://docker.io/library/nginx:1.9.9", 
  "/home/linusberg/Development/apc.test/skopeo/oci_dir");