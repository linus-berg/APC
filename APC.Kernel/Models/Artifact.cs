namespace APC.Kernel.Models;

public class Artifact {
  public Artifact() {
    Versions = new Dictionary<string, ArtifactVersion>();
  }

  public string Id { get; set; }
  public string Type { get; set; }
  public Dictionary<string, ArtifactVersion> Versions { get; set; }

  public bool HasVersion(string version) {
    return Versions.ContainsKey(version);
  }
}