namespace APC.Kernel.Messages; 

public class ArtifactIngestRequest {
  public ArtifactIngestRequest() {
    Artifacts = new HashSet<string>();
  }
  public HashSet<string> Artifacts { get; set; }
  public string Module { get; set; }
}