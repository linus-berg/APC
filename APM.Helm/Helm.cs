using APC.Kernel.Models;
using APM.Helm.Models;
using RestSharp;

namespace APM.Helm;

public class Helm {
  private const string API_ = "https://artifacthub.io/api/v1/packages/helm";
  private readonly RestClient client_ = new(API_);

  public async Task ProcessArtifact(Artifact artifact) {
    await ProcessVersions(artifact);
  }

  private async Task ProcessVersions(Artifact artifact) {
    HelmChartMetadata metadata = await GetMetadata(artifact.id);
    foreach (HelmChartVersion hv in metadata.available_versions) {
      HelmChartMetadata vm = await GetMetadata(artifact.id, hv.version);
      ArtifactVersion version = new();
      version.location = vm.content_url;
      version.version = vm.version;
      artifact.AddVersion(version);

      /* Add required containers */
      AddContainers(artifact, vm.containers_images);
      await AddDependencies(artifact, vm.data);
    }
  }

  private void AddContainers(Artifact artifact,
                             IEnumerable<HelmChartContainerImage> images) {
    foreach (HelmChartContainerImage image in images) {
      artifact.AddDependency(image.image, "container");
    }
  }

  private async Task AddDependencies(Artifact artifact, HelmChartData data) {
    if (data?.dependencies == null) {
      return;
    }

    foreach (HelmChartDependency chart in data.dependencies) {
      if (string.IsNullOrEmpty(chart.repository)) {
        continue;
      }

      Uri uri = new(chart.repository);
      if (uri.Scheme == "file" ||
          string.IsNullOrEmpty(chart.artifacthub_repository_name)) {
        continue;
      }

      artifact.AddDependency(
        $"{chart.artifacthub_repository_name}/{chart.name}",
        artifact.processor);
    }
  }

  private async Task<HelmChartMetadata> GetMetadata(string id, string version) {
    return await GetMetadata($"{id}/{version}");
  }

  private async Task<HelmChartMetadata> GetMetadata(string id) {
    return await client_.GetJsonAsync<HelmChartMetadata>($"/{id}");
  }
}