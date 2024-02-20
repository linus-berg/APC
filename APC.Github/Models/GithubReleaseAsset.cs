namespace APC.Github.Models;

public class GithubReleaseAsset {
  public required string url { get; set; }
  public required string browser_download_url { get; set; }
  public required string name { get; set; }
}