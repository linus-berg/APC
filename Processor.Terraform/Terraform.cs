using Core.Kernel.Exceptions;
using Core.Kernel.Models;
using Processor.Terraform.Models;
using RestSharp;

namespace Processor.Terraform;

public class Terraform : ITerraform {
  private const string C_REGISTRY_ = "https://registry.terraform.io/";
  private readonly RestClient client_ = new(C_REGISTRY_);
  private readonly ILogger<Terraform> logger_;

  public Terraform(ILogger<Terraform> logger) {
    logger_ = logger;
  }

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    ProviderVersions? versions = await GetVersions(artifact.id);
    await ProcessArtifactVersions(artifact, versions);
    return artifact;
  }

  private async Task ProcessArtifactVersions(Artifact artifact,
                                             ProviderVersions? versions) {
    if (versions?.versions == null) {
      return;
    }

    foreach (ProviderVersion v in versions.versions) {
      if (artifact.HasVersion(v.version)) {
        continue;
      }

      ProviderVersionMetadata? metadata =
        await GetMetadata(artifact.id, v.version);

      if (metadata == null) {
        continue;
      }

      ArtifactVersion version = new() {
        version = v.version
      };
      version.AddFile("provider_zip", metadata.download_url);
      version.AddFile("provider_sha", metadata.shasums_url);
      version.AddFile("provider_sig", metadata.shasums_signature_url);
      artifact.AddVersion(version);
    }
  }

  private async Task<ProviderVersions?> GetVersions(string id) {
    try {
      return await client_.GetAsync<ProviderVersions>(
               $"/v1/providers/{id}/versions"
             );
    } catch (TimeoutException ex) {
      logger_.LogError("Timeout error: {Exception}", ex.ToString());
      throw new ArtifactTimeoutException($"{id} timed out!");
    } catch (Exception ex) {
      logger_.LogError("Metadata error: {Exception}", ex.ToString());
      throw new ArtifactMetadataException($"{id} metadata error!");
    }
  }

  private async Task<ProviderVersionMetadata?> GetMetadata(
    string id, string version) {
    try {
      return await client_.GetAsync<ProviderVersionMetadata>(
               $"/v1/providers/{id}/{version}/download/linux/amd64"
             );
    } catch (TimeoutException ex) {
      logger_.LogError("Timeout error: {Exception}", ex.ToString());
      throw new ArtifactTimeoutException($"{id} timed out!");
    } catch (Exception ex) {
      logger_.LogError("Metadata error: {Exception}", ex.ToString());
      throw new ArtifactMetadataException($"{id} metadata error!");
    }
  }
}