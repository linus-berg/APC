namespace APM.Npm.Models;

public class Package {
  public Dictionary<string, string>? dependencies { get; set; }
  public Dictionary<string, string>? peerDependencies { get; set; }
  public Dictionary<string, string>? devDependencies { get; set; }
  public Distribution dist { get; set; }
}