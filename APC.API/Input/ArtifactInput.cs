namespace APC.API.Input;

public class ArtifactInput {
  public required string id { get; set; }
  public required string processor { get; set; }
  public string? filter { get; set; }
  public required Dictionary<string, string> config { get; set; }
}