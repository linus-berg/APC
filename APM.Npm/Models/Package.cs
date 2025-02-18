using System.Text.Json;

namespace APM.Npm.Models;

public class Package {
  public Dictionary<string, JsonElement>? dependencies { get; set; }
  public Dictionary<string, JsonElement>? peerDependencies { get; set; }
  public Dictionary<string, JsonElement>? devDependencies { get; set; }
  public Distribution dist { get; set; }
}