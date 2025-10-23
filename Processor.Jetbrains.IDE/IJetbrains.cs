using Core.Kernel.Models;

namespace Processor.Jetbrains.IDE;

public interface IJetbrains {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}