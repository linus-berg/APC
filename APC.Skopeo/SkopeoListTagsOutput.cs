namespace APC.Skopeo;

public class SkopeoListTagsOutput {
  public required string Repository { get; set; }
  public required IEnumerable<string> Tags { get; set; }
}