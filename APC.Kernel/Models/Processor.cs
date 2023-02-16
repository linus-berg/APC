using MongoDB.Bson;

namespace APC.Kernel.Models;

public class Processor {
  public string Id { get; set; }
  public bool DirectCollect { get; set; } = false;
  
  public string Description { get; set; }
  public BsonDocument Config { get; set; }
}