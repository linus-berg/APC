namespace Library.Skopeo;

public class SkopeoListTagsOutput {
  public string Repository { get; set; }
  public IEnumerable<string> Tags { get; set; }
}