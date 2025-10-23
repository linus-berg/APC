using System.Text.Json.Serialization;

namespace APC.API.Output;

public class KeycloakOptions {
  [JsonPropertyName("auth-server-url")]
  public string url { get; set; }

  public string resource { get; set; }
  public string realm { get; set; }
}