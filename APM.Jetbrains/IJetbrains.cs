using APC.Kernel.Models;
using APM.Jetbrains.Models;

namespace APM.Jetbrains; 

public interface IJetbrains {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}