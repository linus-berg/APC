using System.Text.Json.Serialization;

namespace Processor.OperatorHub.Models;

public class OperatorMetadata {
  [JsonPropertyName("operator")]
  public Operator op { get; set; }
}