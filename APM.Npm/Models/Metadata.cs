namespace APM.Npm.Models;

public class Metadata {
  public required Dictionary<string, Package> versions { get; set; }
}