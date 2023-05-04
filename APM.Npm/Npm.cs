using APC.Kernel.Exceptions;
using APC.Kernel.Models;
using APM.Npm.Models;
using RestSharp;

namespace APM.Npm;

public class Npm : INpm {
  private const string REGISTRY_ = "https://registry.npmjs.org/";
  private readonly RestClient client_ = new(REGISTRY_);

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    Metadata metadata = await GetMetadata(artifact.id);
    ProcessArtifactVersions(artifact, metadata);
    return artifact;
  }

  private void ProcessArtifactVersions(Artifact artifact, Metadata metadata) {
    if (metadata?.versions == null) {
      return;
    }

    foreach (KeyValuePair<string, Package> kv in metadata.versions) {
      if (artifact.HasVersion(kv.Key)) {
        continue;
      }

      Package package = kv.Value;
      ArtifactVersion version = new() {
        version = kv.Key
      };
      version.AddFile(Path.GetFileName(package.dist.tarball), package.dist.tarball);
      AddDependencies(artifact, package.dependencies);
      AddDependencies(artifact, package.peerDependencies);
      artifact.AddVersion(version);
    }
  }

  private void AddDependencies(Artifact artifact,
                               Dictionary<string, string> dependencies) {
    if (dependencies == null) {
      return;
    }

    foreach (KeyValuePair<string, string> package in dependencies) {
      artifact.AddDependency(package.Key, artifact.processor);
    }
  }

  private async Task<Metadata> GetMetadata(string id) {
    try {
      return await client_.GetJsonAsync<Metadata>($"{id}/");
    } catch (TimeoutException ex) {
      throw new ArtifactTimeoutException($"{id} timed out!");
    } catch (Exception ex) {
      throw new ArtifactMetadataException($"{id} metadata error!");
    }
  }
}