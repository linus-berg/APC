using ATM.RKE2.Models;
using RestSharp;

namespace ATM.RKE2; 

public class GithubClient {
  private const string API_ = "https://api.github.com";
  private readonly RestClient client_ = new(API_);
  
  public GithubClient() {
  }

  public async Task<List<GithubRelease>> GetRancherReleases() {
    List<GithubRelease> releases = await GetRancherReleasePage(1);
    return releases.Where(release => !release.prerelease && !release.draft).ToList();
  }

  private async Task<List<GithubRelease>> GetRancherReleasePage(int page) {
    return await client_.GetJsonAsync<List<GithubRelease>>("/repos/rancher/rke2/releases?page={page}&per_page=100", new { page });
  }
}