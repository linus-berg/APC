using APC.Kernel.Models;

namespace APC.Kernel.Messages;

public class ArtifactRouteRequest {
  public required Artifact artifact { get; set; }
}