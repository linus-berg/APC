using APC.Kernel.Exceptions;
using APC.Kernel.Models;
using APM.Pypi.Models;
using RestSharp;

namespace APM.Pypi;

public class Pypi : IPypi {
  private const string REGISTRY_ = "https://pypi.org/";
  private readonly RestClient client_ = new(REGISTRY_);

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    PypiMetadata metadata = await GetMetadata(artifact.id);
    Dictionary<string, List<PypiRelease>> versions =
      metadata.GetAllValidReleases();
    List<string> dependencies = metadata.info.GetDependencies();


    foreach (KeyValuePair<string, List<PypiRelease>> kv in versions) {
      string version = kv.Key;
      List<PypiRelease> releases = kv.Value;
      if (artifact.HasVersion(version)) {
        continue;
      }
      ArtifactVersion a_version = new() {
        version = version
      };
      foreach (PypiRelease release in releases) {
        a_version.AddFile(release.filename, release.url);
      }

      artifact.AddVersion(a_version);
    }

    foreach (string dependency in dependencies) {
      artifact.AddDependency(dependency, artifact.processor);
    }

    return artifact;
  }

  private async Task<PypiMetadata> GetMetadata(string id) {
    try {
      return await client_.GetJsonAsync<PypiMetadata>($"pypi/{id}/json");
    } catch (TimeoutException ex) {
      throw new ArtifactTimeoutException($"{id} timed out!");
    } catch (Exception ex) {
      throw new ArtifactMetadataException($"{id} metadata error!");
    }
  }
}