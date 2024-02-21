using APC.Kernel.Models;

namespace APC.Kernel.Messages;

public class ArtifactIngestRequest {
  public Artifact artifact { get; set; }
}