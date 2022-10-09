using APC.Infrastructure.Models;

namespace APC.Kernel.Messages;

public class ArtifactCollectRequest {
  public Artifact Artifact { get; set; }
}