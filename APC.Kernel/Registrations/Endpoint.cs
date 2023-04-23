namespace APC.Kernel.Registrations;

public class Endpoint {
  public string name { get; set; }
  public int concurrency { get; set; } = 10;
}