using APC.Kernel.Exceptions;
using APC.Kernel.Models;
using APM.Php.Models;
using RestSharp;

namespace APM.Php;

public class Php : IPhp {
  private const string C_REGISTRY_ = "https://repo.packagist.org";
  private const string C_FILE_NAME_ = "zipball";
  private readonly RestClient client_ = new(C_REGISTRY_);
  private readonly ILogger<Php> logger_;

  public Php(ILogger<Php> logger) {
    logger_ = logger;
  }

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    Packages? metadata = await GetMetadata(artifact.id);
    ProcessArtifactVersions(artifact, metadata);
    return artifact;
  }

  private void ProcessArtifactVersions(Artifact artifact, Packages? metadata) {
    if (metadata?.packages == null) {
      return;
    }

    foreach (KeyValuePair<string, List<PackageVersion>> kv in metadata.packages) {
      foreach (PackageVersion package in kv.Value) {
        if (artifact.HasVersion(kv.Key)) {
          continue;
        }
        ArtifactVersion version = new() {
          version = package.version
        };
        version.AddFile(C_FILE_NAME_, package.dist.url);
        AddDependencies(artifact, package.GetRequired());
        AddDependencies(artifact, package.GetRequiredDev());
        artifact.AddVersion(version);
        
      }
    }
  }

  private void AddDependencies(Artifact artifact,
                               Dictionary<string, string>? dependencies) {
    if (dependencies == null) {
      return;
    }

    foreach (KeyValuePair<string, string> package in dependencies) {
      if (package.Key == "php") {
        continue;
      }
      artifact.AddDependency(package.Key, artifact.processor);
    }
  }

  private async Task<Packages?> GetMetadata(string id) {
    try {
      return await client_.GetJsonAsync<Packages>($"p2/{id}.json");
    } catch (TimeoutException ex) {
      logger_.LogError("Timeout error: {Exception}", ex.ToString());
      throw new ArtifactTimeoutException($"{id} timed out!");
    } catch (Exception ex) {
      logger_.LogError("Metadata error: {Exception}", ex.ToString());
      throw new ArtifactMetadataException($"{id} metadata error!");
    }
  }
}