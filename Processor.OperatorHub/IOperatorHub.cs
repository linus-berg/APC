using Core.Kernel.Models;

namespace Processor.OperatorHub;

public interface IOperatorHub {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}