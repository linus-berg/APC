using System.Text.Json.Serialization;

namespace APC.API.Output;

public class KeycloakOptions {
  [JsonPropertyName("auth-server-url")] public required string url { get; set; }

  public required string resource { get; set; }
  public required string realm { get; set; }
}