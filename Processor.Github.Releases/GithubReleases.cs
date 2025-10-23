using System.Text.RegularExpressions;
using Core.Kernel.Models;
using Library.Github;
using Library.Github.Models;

namespace Processor.Github.Releases;

public class GithubReleases : IGithubReleases {
  private readonly IGithubClient gh_;

  public GithubReleases(IGithubClient gh) {
    gh_ = gh;
  }

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    List<GithubRelease> releases = await gh_.GetReleases(artifact.id);
    List<Regex> files_regexp =
      artifact.config["files"].Split(";").Select(r => new Regex(r)).ToList();

    foreach (GithubRelease release in releases) {
      ArtifactVersion version = new() {
        version = release.tag_name
      };
      foreach (GithubReleaseAsset asset in release.assets) {
        foreach (Regex file_regexp in files_regexp) {
          if (file_regexp.IsMatch(asset.name)) {
            string url = asset.browser_download_url;
            version.AddFile(Path.GetFileName(url), url);
          }
        }
      }

      artifact.AddVersion(version);
    }

    return artifact;
  }
}