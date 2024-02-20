namespace APM.Maven.Models;

public class MavenSearchDoc {
  public required string id { get; set; }
  public required string g { get; set; }
  public required string v { get; set; }
  public required List<string> ec { get; set; }
}