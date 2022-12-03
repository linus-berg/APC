using APC.Kernel.Models;

namespace APM.Npm;

public interface INpm {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}