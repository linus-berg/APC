using System.Text.Json.Serialization;
using MassTransit.Futures.Contracts;

namespace APM.OperatorHub.Models;

public class OperatorMetadata {
  [JsonPropertyName("operator")]
  public Operator op { get; set; }
}