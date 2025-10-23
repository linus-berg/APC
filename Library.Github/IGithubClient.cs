using Library.Github.Models;

namespace Library.Github;

public interface IGithubClient {
  public Task<List<GithubRelease>> GetReleases(string repo);
}