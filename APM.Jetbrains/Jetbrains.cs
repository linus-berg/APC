using APC.Kernel.Exceptions;
using APC.Kernel.Models;
using APM.Jetbrains.Models;
using RestSharp;

namespace APM.Jetbrains; 

public class Jetbrains : IJetbrains {
  private const string API_ = "https://plugins.jetbrains.com";
  private readonly RestClient client_ = new(API_);
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
      ArtifactVersion version = new ArtifactVersion() {
        version = update.version
      };
      version.AddFile("plugin", $"{API_}/files/{update.file}", "jetbrains");
      artifact.AddVersion(version);
    }
  }

  private async Task<List<JetbrainsPluginUpdate>?> GetUpdates(string id) {
    try {
      return await client_.GetJsonAsync<List<JetbrainsPluginUpdate>>($"/api/plugins/{id}/updates");
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