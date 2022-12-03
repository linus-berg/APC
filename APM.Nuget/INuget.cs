using APC.Kernel.Models;

namespace APM.Nuget;

public interface INuget {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}