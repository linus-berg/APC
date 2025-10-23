using Core.Kernel.Models;

namespace Processor.Jetbrains.Plugins;

public interface IJetbrains {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}