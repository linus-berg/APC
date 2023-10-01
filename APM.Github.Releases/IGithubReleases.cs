using APC.Kernel.Models;

namespace APM.Github.Releases;

public interface IGithubReleases {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}