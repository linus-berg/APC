namespace APC.Kernel.Messages;

public class ArtifactProcessRequest {
  public Guid Context { get; set; }
  public string Name { get; set; }
  public string Module { get; set; }
  public string Filter { get; set; }
}