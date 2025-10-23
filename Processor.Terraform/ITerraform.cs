using Core.Kernel.Models;

namespace Processor.Terraform;

public interface ITerraform {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}