namespace APC.Kernel.Models; 

public record ArtifactFile {
  public string uri { get; set; }
  public string processor { get; set; }
}