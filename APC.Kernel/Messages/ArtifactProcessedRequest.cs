using APC.Services.Models;

namespace APC.Kernel.Messages;

public class ArtifactProcessedRequest {
  public ArtifactProcessedRequest() {
    CollectRequests = new List<ArtifactCollectRequest>();
  }

  public List<ArtifactCollectRequest> CollectRequests { get; set; }

  public Guid Context { get; set; }
  public Artifact Artifact { get; set; }

  public void AddCollectRequest(string location, string module) {
    CollectRequests.Add(new ArtifactCollectRequest {
      module = module,
      location = location
    });
  }
}