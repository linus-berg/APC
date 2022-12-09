using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace APC.Kernel.Models; 

public class Processor {
  public string Id { get; set; }
  
  [BsonExtraElements]
  public BsonDocument Config { get; set; }
}