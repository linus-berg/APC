using APC.Github.Models;

namespace APC.Github;

public interface IGithubClient {
  public Task<List<GithubRelease>> GetReleases(string repo);
}