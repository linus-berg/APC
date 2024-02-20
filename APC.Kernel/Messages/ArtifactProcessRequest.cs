using APC.Kernel.Models;

namespace APC.Kernel.Messages;

public class ArtifactProcessRequest {
  public Guid ctx { get; set; }
  public required Artifact artifact { get; set; }
}