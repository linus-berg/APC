using APC.Kernel.Models;

namespace APM.Terraform;

public interface ITerraform {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
}