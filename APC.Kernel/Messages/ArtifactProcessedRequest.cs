using APC.Services.Models;

namespace APC.Kernel.Messages;

public class ArtifactProcessedRequest {
  public List<string> DirectCollect;

  public ArtifactProcessedRequest() {
    DirectCollect = new List<string>();
  }

  public Guid Context { get; set; }
  public Artifact Artifact { get; set; }
}