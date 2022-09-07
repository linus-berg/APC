namespace APC.Kernel.Models;

public class ArtifactVersion {
  public ArtifactVersion() {
    Dependencies = new HashSet<string>();
  }

  public string Id { get; set; }
  public string Version { get; set; }
  public HashSet<string> Dependencies { get; set; }
  public string Uri { get; set; }

  public bool AddDependency(string id) {
    return Dependencies.Add(id);
  }
}