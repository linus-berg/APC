namespace APC.Kernel.Models;

public class ArtifactProcessingFault {
  public int id { get; set; }
  public required string name { get; set; }
  public required string processor { get; set; }
  public DateTime time { get; set; }
}