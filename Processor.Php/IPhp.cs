using Core.Kernel.Models;

namespace Processor.Php;

public interface IPhp {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}