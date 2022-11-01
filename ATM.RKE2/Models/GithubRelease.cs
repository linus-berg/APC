using MassTransit.Futures.Contracts;

namespace ATM.RKE2.Models; 

public class GithubRelease {
  public string url { get; set; }
  public string html_url { get; set; }
  public string assets_url { get; set; }
  public string upload_url { get; set; }
  public string tarball_url { get; set; }
  public string zipball_url { get; set; }
  public int id { get; set; }
  public string node_id { get; set; }
  public string tag_name { get; set; }
  public string target_commitish { get; set; }
  public string name { get; set; }
  public bool draft { get; set; }
  public bool prerelease { get; set; }
  public List<GithubReleaseAsset> assets { get; set; }

  public string GetRancherImageFile() {
    foreach (GithubReleaseAsset asset in assets) {
      if (asset.name == "rke2-images-all.linux-amd64.txt") {
        return asset.browser_download_url;
      }
    }
    return null;
  }
}