using APC.Kernel.Exceptions;
using APC.Kernel.Models;
using APM.Jetbrains.IDE.Models;
using RestSharp;

namespace APM.Jetbrains.IDE;

public class Jetbrains : IJetbrains {
  private const string API_ = "https://data.services.jetbrains.com";
  private readonly RestClient client_ = new(API_);

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    string id = artifact.id;
    JetbrainsProduct product = await GetProduct(id);
    AddVersions(artifact, product);

    return artifact;
  }

  private void AddVersions(Artifact artifact, JetbrainsProduct product) {
    foreach (JetbrainsProductRelease release in product.releases) {
      Dictionary<string, JetbrainsProductDownload>
        downloads = release.downloads;
      if (downloads.TryGetValue("linux",
                                out JetbrainsProductDownload? download)) {
        ArtifactVersion version = new() {
          version = release.version
        };
        version.AddFile("linux", download.link);
        artifact.AddVersion(version);
      }
    }
  }

  private async Task<JetbrainsProduct> GetProduct(string id) {
    try {
      List<JetbrainsProduct>? products =
        await client_.GetJsonAsync<List<JetbrainsProduct>>(
          $"/products?code={id}&release.type=release");

      if (products == null || products.Count == 0) {
        throw new ArtifactMetadataException($"{id} had no products in list");
      }

      return products[0];
    } catch (TimeoutException ex) {
      throw new ArtifactTimeoutException($"{id} timed out!");
    } catch (Exception ex) {
      throw new ArtifactMetadataException($"{id} metadata error!");
    }
  }
}