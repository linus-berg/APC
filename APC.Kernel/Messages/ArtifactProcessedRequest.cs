using APC.Kernel.Models;

namespace APC.Kernel.Messages;

public class ArtifactProcessedRequest {
  public Artifact Artifact { get; set; }
}