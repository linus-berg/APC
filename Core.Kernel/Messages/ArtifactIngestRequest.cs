using Core.Kernel.Models;

namespace Core.Kernel.Messages;

public class ArtifactIngestRequest {
  public Artifact artifact { get; set; }
}