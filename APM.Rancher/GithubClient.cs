using APM.Rancher.Models;
using RestSharp;

namespace APM.Rancher;

public class GithubClient : IGithubClient {
  private const string API_ = "https://api.github.com";
  private readonly RestClient client_ = new(API_);

  public async Task<List<GithubRelease>> GetRancherReleases(string repo) {
    List<GithubRelease> releases = await GetRancherReleasePage(repo, 1);
    return releases.Where(release => !release.prerelease && !release.draft)
                   .ToList();
  }

  private async Task<List<GithubRelease>> GetRancherReleasePage(string repo, int page) {
    return await client_.GetJsonAsync<List<GithubRelease>>(
             $"/repos/{repo}/releases?page={page}&per_page=100",
             new {
               page
             });
  }
}