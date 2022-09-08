using APC.Kernel.Models;
using APM.Npm.Models;
using RestSharp;

namespace APM.Npm;

public class Npm : INpm {
  private const string REGISTRY_ = "https://registry.npmjs.org/";
  private readonly RestClient client_ = new(REGISTRY_);

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    Metadata metadata = await GetMetadata(artifact.Id);
    if (metadata.versions.Count == artifact.Versions.Count) {
      return artifact;
    }

    ProcessArtifactVersions(artifact, metadata);
    return artifact;
  }

  private void ProcessArtifactVersions(Artifact artifact,
    Metadata metadata) {
    foreach (KeyValuePair<string, Package> kv in metadata.versions) {
      if (artifact.HasVersion(kv.Key)) {
        continue;
      }

      Package package = kv.Value;
      ArtifactVersion version = new ArtifactVersion() {
        Id = artifact.Id,
        Uri = package.dist.tarball,
        Version = kv.Key
      };
      AddDependencies(version, package.dependencies);
      AddDependencies(version, package.peerDependencies);
      artifact.AddVersion(version);
    }
  }

  private void AddDependencies(ArtifactVersion version, Dictionary<string, string> dependencies) {
    if (dependencies == null) return;
    foreach (KeyValuePair<string, string> package in dependencies)
      version.AddDependency(package.Key);
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