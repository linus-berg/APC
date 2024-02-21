using MongoDB.Bson.Serialization.Attributes;

namespace APC.Kernel.Models;

public class Processor {
  public string id { get; set; }
  public bool direct_collect { get; set; } = false;

  public string description { get; set; }

  public Dictionary<string, ProcessorAuxiliaryField> config { get; set; }
}