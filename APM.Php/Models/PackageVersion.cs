using System.Text.Json;
using System.Text.Json.Serialization;

namespace APM.Php.Models;

public class PackageVersion {
  public PackageDist dist { get; set; }
  public string version { get; set; }
  public JsonElement require { get; set; }

  [JsonPropertyName("require-dev")]
  public JsonElement require_dev { get; set; }
  //public Dictionary<string, string> require_dev { get; set; }

  public Dictionary<string, string> GetRequired() {
    if (require.ValueKind != JsonValueKind.Object) {
      return new Dictionary<string, string>();
    }

    try {
      return require_dev.Deserialize<Dictionary<string, string>>();
    } catch {
      return new Dictionary<string, string>();
    }
  }

  public Dictionary<string, string> GetRequiredDev() {
    if (require.ValueKind != JsonValueKind.Object) {
      return new Dictionary<string, string>();
    }

    try {
      return require_dev.Deserialize<Dictionary<string, string>>();
    } catch {
      return new Dictionary<string, string>();
    }
  }
}