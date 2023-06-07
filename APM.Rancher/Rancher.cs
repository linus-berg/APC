using APC.Kernel;
using APC.Kernel.Models;
using APM.Rancher.Models;
using RestSharp;

namespace APM.Rancher;

public class Rancher : IRancher {
  private readonly IGithubClient gh_;
  private readonly RestClient http_client_ = new();

  public Rancher(IGithubClient gh) {
    gh_ = gh;
  }

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    List<GithubRelease> releases = await gh_.GetRancherReleases(artifact.id);
    string file = artifact.config["file"];
    
    foreach (GithubRelease release in releases) {
      if (artifact.HasVersion(release.tag_name)) {
        continue;
      }
      
      string url = release.GetRancherImageFile(file);
      string filename = $"{release.tag_name}_{Path.GetFileName(url)}";
      if (url == null) {
        continue;
      }

      ArtifactVersion version = new ArtifactVersion() {
        version = release.tag_name
      };

      Console.WriteLine($"New release found {url}");
      string rancher_file = await GetRancherFile(url, filename);
      string[] images = rancher_file.Split('\n');
      foreach (string image in images) {
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

  private async Task<string> GetRancherFile(string url, string filename) {
    return await http_client_.GetAsync<string>(new RestRequest(url));
  }
}