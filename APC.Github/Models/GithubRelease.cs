using System.Text.RegularExpressions;

namespace APC.Github.Models;

public class GithubRelease {
  public required string url { get; set; }
  public required string html_url { get; set; }
  public required string assets_url { get; set; }
  public required string upload_url { get; set; }
  public required string tarball_url { get; set; }
  public required string zipball_url { get; set; }
  public int id { get; set; }
  public required string node_id { get; set; }
  public required string tag_name { get; set; }
  public required string target_commitish { get; set; }
  public required string name { get; set; }
  public bool draft { get; set; }
  public bool prerelease { get; set; }
  public required List<GithubReleaseAsset> assets { get; set; }

  public string GetReleaseFileRegexp(Regex regex) {
    foreach (GithubReleaseAsset asset in assets) {
      if (regex.IsMatch(asset.name)) {
        return asset.browser_download_url;
      }
    }

    return null;
  }

  public string GetReleaseFile(string name) {
    foreach (GithubReleaseAsset asset in assets) {
      if (asset.name == name) {
        return asset.browser_download_url;
      }
    }

    return null;
  }
}