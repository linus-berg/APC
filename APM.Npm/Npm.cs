using APC.Infrastructure.Models;
using APC.Kernel.Exceptions;
using APM.Npm.Models;
using RestSharp;

namespace APM.Npm;

public class Npm : INpm {
  private const string REGISTRY_ = "https://registry.npmjs.org/";
  private readonly RestClient client_ = new(REGISTRY_);

  public async Task<Artifact> ProcessArtifact(string name) {
    Metadata metadata = await GetMetadata(name);
    if (metadata == null) throw new ArtifactMetadataException($"Could not get metadata: {name}");
    Artifact artifact = new() {
      name = name,
      module = "npm"
    };
    ProcessArtifactVersions(artifact, metadata);
    return artifact;
  }

  private void ProcessArtifactVersions(Artifact artifact,
    Metadata metadata) {
    foreach (KeyValuePair<string, Package> kv in metadata.versions) {
      if (artifact.HasVersion(kv.Key)) continue;

      Package package = kv.Value;
      ArtifactVersion version = new() {
        artifact_id = artifact.id,
        location = package.dist.tarball,
        version = kv.Key
      };
      AddDependencies(artifact, version, package.dependencies);
      AddDependencies(artifact, version, package.peerDependencies);
      artifact.AddVersion(version);
    }
  }

  private void AddDependencies(Artifact artifact, ArtifactVersion version, Dictionary<string, string> dependencies) {
    if (dependencies == null) return;
    foreach (KeyValuePair<string, string> package in dependencies) {
      artifact.AddDependency(package.Key);
      version.AddDependency(package.Key);
    }
  }

  private async Task<Metadata> GetMetadata(string id) {
    try {
      return await client_.GetJsonAsync<Metadata>($"{id}/");
    }
    catch (Exception ex) {
      return null;
    }
  }
}