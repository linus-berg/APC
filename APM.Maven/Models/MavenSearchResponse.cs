namespace APM.Maven.Models;

public class MavenSearchResponse {
  public int numFound { get; set; }
  public int start { get; set; }
  public List<MavenSearchDoc> docs { get; set; }
}