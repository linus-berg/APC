using ATM.Rancher.Models;
using RestSharp;

namespace ATM.Rancher; 

public class GithubClient {
  private const string API_ = "https://api.github.com";
  private readonly RestClient client_ = new(API_);
  private readonly string repo_;
  
  public GithubClient(string repo) {
    repo_ = repo;
  }

  public async Task<List<GithubRelease>> GetRancherReleases() {
    List<GithubRelease> releases = await GetRancherReleasePage(1);
    return releases.Where(release => !release.prerelease && !release.draft).ToList();
  }

  private async Task<List<GithubRelease>> GetRancherReleasePage(int page) {
    return await client_.GetJsonAsync<List<GithubRelease>>($"/repos/{repo_}/releases?page={page}&per_page=100", new { page });
  }
}