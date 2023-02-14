// See https://aka.ms/new-console-template for more information

using ACM.Git;
using APC.Skopeo;

SkopeoClient client = new SkopeoClient();



List<string> containers = new List<string>() {
  "docker://docker.io/library/nginx:1.23-alpine",
  "docker://docker.io/nginx:1.23-alpine"
};

foreach (string container in containers) {
  SkopeoManifest? manifest = await client.ImageExists(
                               container,
                               "/home/bfde19/Development/skopeo");
  if (manifest == null) {
    Console.WriteLine($"{container} does not exist");
  } else {
    Console.WriteLine($"{container} EXISTS!");
    Console.WriteLine($"{manifest.VerifyLayers()}");
  }
}
