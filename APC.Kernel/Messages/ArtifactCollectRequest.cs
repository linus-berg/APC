namespace APC.Kernel.Messages;

public class ArtifactCollectRequest {
  public required string location { get; set; }
  public required string module { get; set; }

  public bool force { get; set; } = false;

  public string GetCollectorModule() {
    Uri uri = new(location);
    string scheme = uri.Scheme;
    return $"acm-{scheme}";
  }
}