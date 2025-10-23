using APM.Jetbrains.Models;
using Core.Kernel.Exceptions;
using Core.Kernel.Models;
using RestSharp;

namespace APM.Jetbrains;

public class Jetbrains : IJetbrains {
  private const string C_API_ = "https://plugins.jetbrains.com";
  private readonly RestClient client_ = new(C_API_);

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    string id = GetPluginId(artifact.id);
    List<JetbrainsPluginUpdate>? updates = await GetUpdates(id);
    if (updates == null) {
      return artifact;
    }

    AddVersions(artifact, updates);

    return artifact;
  }

  private void AddVersions(Artifact artifact,
                           List<JetbrainsPluginUpdate> updates) {
    foreach (JetbrainsPluginUpdate update in updates) {
      ArtifactVersion version = new() {
        version = update.version
      };
      version.AddFile("plugin", $"{C_API_}/files/{update.file}");
      artifact.AddVersion(version);
    }
  }

  private async Task<List<JetbrainsPluginUpdate>?> GetUpdates(string id) {
    try {
      return await client_.GetJsonAsync<List<JetbrainsPluginUpdate>>(
               $"/api/plugins/{id}/updates"
             );
    } catch (TimeoutException ex) {
      throw new ArtifactTimeoutException($"{id} timed out!");
    } catch (Exception ex) {
      throw new ArtifactMetadataException($"{id} metadata error!");
    }
  }

  private string GetPluginId(string full_id) {
    return full_id.Split('-')[0];
  }
}