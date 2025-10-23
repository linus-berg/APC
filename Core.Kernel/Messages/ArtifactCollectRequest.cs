namespace Core.Kernel.Messages;

public class ArtifactCollectRequest {
  public string location { get; set; }
  public string module { get; set; }

  public bool force { get; set; } = false;

  public string GetCollectorModule() {
    Uri uri = new(location);
    string scheme = uri.Scheme;
    return $"collector-{scheme}";
  }
}