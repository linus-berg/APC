namespace Core.Infrastructure.Models;

public class ArtifactSummary {
  public string id { get; set; }
  public string processor { get; set; }
  public string filter { get; set; }
  public bool root { get; set; }
  public Dictionary<string, string> config { get; set; }
  public int versions { get; set; }
  public int dependencies { get; set; }
  
}