using System.Text.RegularExpressions;
using APC.Kernel.Models;
using APM.Helm.Models;
using RestSharp;

namespace APM.Helm;

public class Helm {
  private const string API_ = "https://artifacthub.io/api/v1/packages/helm";
  private readonly RestClient client_ = new(API_);

  public Helm() {
    AddApiKeyIfAvailable();
  }
  
  private void AddApiKeyIfAvailable() {
    string? api_key_id =
      Environment.GetEnvironmentVariable("ARTIFACTHUB_API_KEY_ID");
    string? api_key_secret =
      Environment.GetEnvironmentVariable("ARTIFACTHUB_API_KEY_SECRET");
    if (!string.IsNullOrEmpty(api_key_id) &&
        !string.IsNullOrEmpty(api_key_secret)) {
      client_.AddDefaultHeader("X-API-KEY-ID", api_key_id);
      client_.AddDefaultHeader("X-API-KEY-SECRET", api_key_secret);
    }
  }

  public async Task ProcessArtifact(Artifact artifact) {
    await ProcessVersions(artifact);
  }

  private async Task ProcessVersions(Artifact artifact) {
    HelmChartMetadata metadata = await GetMetadata(artifact.id);
    foreach (HelmChartVersion hv in metadata.available_versions) {
      if (artifact.HasVersion(hv.version)) {
        continue;
      }
      HelmChartMetadata vm = await GetMetadata(artifact.id, hv.version);
      ArtifactVersion version = new();
      version.AddFile("chart", vm.content_url);
      version.version = vm.version;

      /* Add required containers */
      AddContainers(version, vm.containers_images);
      artifact.AddVersion(version);
      AddDependencies(artifact, vm.data);
    }
  }

  private void AddContainers(ArtifactVersion artifact_version,
                             IEnumerable<HelmChartContainerImage> images) {
    foreach (HelmChartContainerImage image in images) {
      artifact_version.AddFile($"{image.image}", FixNaming(image.image),
                               "container");
    }
  }

  private static string FixNaming(string name) {
    return !HasHostname(name)
             ? $"docker://docker.io/{name}"
             : $"docker://{name}";
  }

  private static bool HasHostname(string name) {
    bool is_match = Regex.IsMatch(name, @"\w+\.\w+\/");
    return is_match;
  }

  private void AddDependencies(Artifact artifact, HelmChartData data) {
    if (data?.dependencies == null) {
      return;
    }
    foreach (HelmChartDependency chart in data.dependencies) {
      TryAddDependency(artifact, chart);
    }
  }

  private bool TryAddDependency(Artifact artifact, HelmChartDependency chart) {
    try {
      AddDependency(artifact, chart);
    } catch (Exception e) {
      Console.WriteLine(e);
      return false;
    }
    return true;
  }

  private bool AddDependency(Artifact artifact, HelmChartDependency chart) {
      if (string.IsNullOrEmpty(chart.repository)) {
        return false;
      }

      Uri uri = new(chart.repository);
      if (uri.Scheme == "file" ||
          string.IsNullOrEmpty(chart.artifacthub_repository_name)) {
        return false;
      }

      artifact.AddDependency(
        $"{chart.artifacthub_repository_name}/{chart.name}",
        artifact.processor);
      return true; 
    
  }

  private async Task<HelmChartMetadata> GetMetadata(string id, string version) {
    return await GetMetadata($"{id}/{version}");
  }

  private async Task<HelmChartMetadata> GetMetadata(string id) {
    return await client_.GetJsonAsync<HelmChartMetadata>($"/{id}");
  }
}