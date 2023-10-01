using APC.Kernel.Models;

namespace APM.Rancher;

public interface IRancher {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}