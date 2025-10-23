using Core.Kernel.Models;

namespace Processor.Github.Releases;

public interface IGithubReleases {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}