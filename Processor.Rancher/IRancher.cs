using Core.Kernel.Models;

namespace Processor.Rancher;

public interface IRancher {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}