namespace APC.Kernel.Messages;

public class ArtifactCollectRequest {
  public string location { get; set; }
  public string module { get; set; }

  public string GetCollectorModule() {
    Uri uri = new(location);
    string scheme = uri.Scheme;
    return $"acm-{scheme}";
  }
}