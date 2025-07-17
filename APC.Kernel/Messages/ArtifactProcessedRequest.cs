using APC.Kernel.Models;

namespace APC.Kernel.Messages;

public class ArtifactProcessedRequest {
  public ArtifactProcessedRequest() {
    collect_requests = new List<ArtifactCollectRequest>();
  }

  public List<ArtifactCollectRequest> collect_requests { get; set; }

  public Guid context { get; set; }
  public Artifact artifact { get; set; }

  public void AddCollectRequest(string location, string module,
                                bool force = false) {
    collect_requests.Add(
      new ArtifactCollectRequest {
        module = module,
        location = location,
        force = force
      }
    );
  }
}