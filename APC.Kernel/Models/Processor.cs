namespace APC.Kernel.Models;

public class Processor {
  public required string id { get; set; }
  public bool direct_collect { get; set; } = false;

  public required string description { get; set; }

  public required Dictionary<string, ProcessorAuxiliaryField> config {
    get;
    set;
  }
}