using APM.Rancher.Models;

namespace APM.Rancher; 

public interface IGithubClient {
  public Task<List<GithubRelease>> GetRancherReleases(string repo);
}