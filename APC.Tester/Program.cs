// See https://aka.ms/new-console-template for more information

using APC.Infrastructure;
using APC.Services;
using APC.Services.Models;

IApcDatabase mongo = new ApcDatabase();
Artifact artifact = new() {
  id = "react",
  module = "npm"
};

artifact.root = true;
ArtifactVersion version = new();

version.location = "http://test.com";
version.version = "1.0.0";
artifact.AddVersion(version);
ArtifactVersion version2 = new();

await mongo.AddArtifact(artifact);

version2.location = "http://testble.com";
version2.version = "2.0.0";
artifact.AddVersion(version2);

await mongo.UpdateArtifact(artifact);
artifact.AddVersion(version2);
await mongo.UpdateArtifact(artifact);