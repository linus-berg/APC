using APC.Kernel.Models;

namespace APM.OperatorHub;

public interface IOperatorHub {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}