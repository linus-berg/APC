using APC.Services.Models;

namespace APC.Kernel.Messages;

public class ArtifactProcessedRequest {
  public Guid Context { get; set; }
  public Artifact Artifact { get; set; }
}