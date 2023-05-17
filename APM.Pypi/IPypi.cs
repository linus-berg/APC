using APC.Kernel.Models;

namespace APM.Pypi;

public interface IPypi {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}