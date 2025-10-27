namespace Core.Kernel.Models;

public class QueueStatus {
  public required string name { get; set; }
  public required long messages { get; set; } = 0;
  public required int consumers { get; set; } = 0;
  public required double? avg_egress_rate { get; set; } = 0;
  public required double? avg_ingress_rate { get; set; } = 0;
}