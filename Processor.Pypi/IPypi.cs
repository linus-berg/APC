using Core.Kernel.Models;

namespace Processor.Pypi;

public interface IPypi {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}