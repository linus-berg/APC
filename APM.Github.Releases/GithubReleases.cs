using System.Text.RegularExpressions;
using APC.Github;
using APC.Github.Models;
using APC.Kernel.Exceptions;
using APC.Kernel.Models;
using RestSharp;

namespace APM.Github.Releases;

public class GithubReleases : IGithubReleases {
  private readonly IGithubClient gh_;

  public GithubReleases(IGithubClient gh) {
    gh_ = gh;
  }

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    List<GithubRelease> releases = await gh_.GetReleases(artifact.id);
    string[] files = artifact.config["files"].Split(";");

    foreach (GithubRelease release in releases) {
      if (artifact.HasVersion(release.tag_name)) {
        continue;
      }


      ArtifactVersion version = new() {
        version = release.tag_name
      };
      foreach (string file in files) {
        string? url = release.GetReleaseFileRegexp(new Regex(file));
        if (string.IsNullOrEmpty(url)) {
          continue;
        }
        version.AddFile(Path.GetFileName(url), url);
      }
      artifact.AddVersion(version);
    }

    return artifact;
  }
}