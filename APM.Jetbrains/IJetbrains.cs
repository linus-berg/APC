using APC.Kernel.Models;

namespace APM.Jetbrains;

public interface IJetbrains {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}