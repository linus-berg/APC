using Core.Kernel.Models;

namespace Processor.Nuget;

public interface INuget {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}