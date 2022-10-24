using APC.Services.Models;

namespace APC.Kernel.Messages;

public class ArtifactCollectRequest {
  public string location { get; set; }
  public string module { get; set; }
}