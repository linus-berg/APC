namespace APM.Npm.Models;

public class Package {
  public required Dictionary<string, string> dependencies { get; set; }
  public required Dictionary<string, string> peerDependencies { get; set; }
  public required Dictionary<string, string> devDependencies { get; set; }
  public required Distribution dist { get; set; }
}