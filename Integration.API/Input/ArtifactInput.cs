namespace Integration.API.Input;

public class ArtifactInput {
  public string id { get; set; }
  public string processor { get; set; }
  public string? filter { get; set; }
  public Dictionary<string, string> config { get; set; }
}