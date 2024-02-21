namespace APC.Kernel.Models;

public class ArtifactProcessingFault {
  public int id { get; set; }
  public string name { get; set; }
  public string processor { get; set; }
  public DateTime time { get; set; }
}