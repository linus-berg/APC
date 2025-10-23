using Core.Kernel.Models;

namespace Processor.Npm;

public interface INpm {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}