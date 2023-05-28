// See https://aka.ms/new-console-template for more information

using APC.Kernel.Models;
using APC.Skopeo;
using APM.Pypi;

Pypi pypi = new();
SkopeoClient s = new();

Artifact artifact = new() {
  id = "pandas",
  processor = "pypi"
};
string image = "quay.io/minio/minio";
string dir = "/home/linusberg/Development/container";
SkopeoListTagsOutput tags = await s.GetTags(image);

foreach (string tag in tags.Tags) {
  SkopeoManifest manifest =
    await s.ImageExists($"docker://{image}:{tag}", dir);
  if (manifest == null) {
    await s.CopyToOci($"docker://{image}:{tag}", dir);
  } else {
    bool complete = manifest.VerifyLayers();
    Console.WriteLine($"{tag} exists!");
  }
}

//Artifact p = await pypi.ProcessArtifact(artifact);
Console.WriteLine("---");