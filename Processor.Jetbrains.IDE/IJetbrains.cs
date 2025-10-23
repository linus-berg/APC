using Core.Kernel.Models;

namespace APM.Jetbrains.IDE;

public interface IJetbrains {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}