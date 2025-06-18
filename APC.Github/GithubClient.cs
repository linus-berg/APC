using APC.Github.Models;
using RestSharp;

namespace APC.Github;

public class GithubClient : IGithubClient {
  private const string C_API_ = "https://api.github.com";
  private readonly RestClient client_ = new(C_API_);

  private const int C_PAGE_SIZE = 10;
  private const int C_MAX_RELEASES = 100;

  public async Task<List<GithubRelease>> GetReleases(string repo)
  {
    List<GithubRelease> releases = new List<GithubRelease>();

    for (int page = 1; releases.Count < C_MAX_RELEASES; page++)
    {
      List<GithubRelease> page_releases = await GetReleasePage(repo, page, C_PAGE_SIZE);

      if (page_releases.Count == 0)
      {
        break;
      }

      releases.AddRange(page_releases.Where(release => !release.prerelease && !release.draft));
    }

    return releases;
  }


  private async Task<List<GithubRelease>>
    GetReleasePage(string repo, int page, int page_size = 10) {
    try {
      return await client_.GetJsonAsync<List<GithubRelease>>(
               $"/repos/{repo}/releases?page={page}&per_page={page_size}",
               new {
                 page
               });
    } catch (Exception e) {
      Console.WriteLine(e);
      throw;
    }
  }
}