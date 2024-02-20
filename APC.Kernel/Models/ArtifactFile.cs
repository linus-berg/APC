namespace APC.Kernel.Models;

public record ArtifactFile {
  public required string uri { get; set; }
  public string folder { get; set; } = "";
}