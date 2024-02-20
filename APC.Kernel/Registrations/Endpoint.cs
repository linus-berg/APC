namespace APC.Kernel.Registrations;

public class Endpoint {
  public required string name { get; set; }
  public int concurrency { get; set; } = 10;
}