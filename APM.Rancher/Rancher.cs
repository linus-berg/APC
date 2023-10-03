using APC.Github;
using APC.Github.Models;
using APC.Kernel.Exceptions;
using APC.Kernel.Models;
using RestSharp;

namespace APM.Rancher;

public class Rancher : IRancher {
  private readonly IGithubClient gh_;
  private readonly RestClient http_client_ = new();
  private readonly ILogger<Rancher> logger_;

  public Rancher(IGithubClient gh, ILogger<Rancher> logger) {
    gh_ = gh;
    logger_ = logger;
  }

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    List<GithubRelease> releases = await gh_.GetReleases(artifact.id);
    string file = artifact.config["file"];

    foreach (GithubRelease release in releases) {
      if (artifact.HasVersion(release.tag_name)) {
        continue;
      }

      string url = release.GetReleaseFile(file);
      if (url == null) {
        continue;
      }

      ArtifactVersion version = new() {
        version = release.tag_name
      };
      logger_.LogInformation($"Release detected={url}");
      string? rancher_file = await GetRancherFile(url);
      if (rancher_file == null) {
        throw new ArtifactMetadataException($"Could not get {url}");
      }

      string[] images = rancher_file.Split('\n');
      foreach (string image in images) {
        if (string.IsNullOrEmpty(image)) {
          continue;
        }

        string clean =
          image.Contains("docker.io")
            ? image
            : $"docker.io/{image}";
        version.AddFile(image, $"docker://{clean}");
      }

      artifact.AddVersion(version);
    }

    return artifact;
  }

  private async Task<string?> GetRancherFile(string url) {
    RestResponse response =
      await http_client_.GetAsync(new RestRequest(url));
    return response.Content ?? null;
  }
}