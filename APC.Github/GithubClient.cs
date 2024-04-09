using System.Net;
using APC.Github.Models;
using RestSharp;

namespace APC.Github;

public class GithubClient : IGithubClient {
  private const string C_API_ = "https://api.github.com";
  private readonly RestClient client_ = new(C_API_);

  public async Task<List<GithubRelease>> GetReleases(string repo) {
    List<GithubRelease> releases = await GetReleasePage(repo, 1);
    return releases.Where(release => !release.prerelease && !release.draft)
                   .ToList();
  }
  

  private async Task<List<GithubRelease>>
    GetReleasePage(string repo, int page) {
    try {
      return await client_.GetJsonAsync<List<GithubRelease>>(
               $"/repos/{repo}/releases?page={page}&per_page=100",
               new {
                 page
               });
    } catch (Exception e) {
      Console.WriteLine(e);
      throw;
    }
  }
}