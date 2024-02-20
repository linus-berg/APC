using APC.Kernel.Models;

namespace APC.Kernel.Messages;

public class ArtifactIngestRequest {
  public required Artifact artifact { get; set; }
}