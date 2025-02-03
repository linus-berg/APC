namespace APC.API.Output;

public class ArtifactOutput {
  public string id { get; set; }
  public string processor { get; set; }
  public string filter { get; set; }
  public bool root { get; set; }
  public int versions { get; set; }
  public int dependencies { get; set; }
}