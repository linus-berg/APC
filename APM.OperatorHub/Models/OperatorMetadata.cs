using System.Text.Json.Serialization;

namespace APM.OperatorHub.Models;

public class OperatorMetadata {
  [JsonPropertyName("operator")]
  public Operator op { get; set; }
}