namespace APC.API.Input;

public class ArtifactInput {
  public string Id { get; set; }
  public string Processor { get; set; }
  public string? Filter { get; set; }
  public Dictionary<string, string> Config { get; set; }
}