using Core.Kernel.Models;

namespace Core.Kernel.Messages;

public class ArtifactProcessRequest {
  public Guid ctx { get; set; }
  public Artifact artifact { get; set; }
}