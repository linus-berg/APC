using APC.Kernel.Models;

namespace APC.Kernel.Messages;

public class ArtifactProcessRequest {
  public ArtifactProcessRequest() {
  }

  public Artifact Artifact { get; set; }
}