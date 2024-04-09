using APC.Kernel.Models;

namespace APM.Php;

public interface IPhp {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}