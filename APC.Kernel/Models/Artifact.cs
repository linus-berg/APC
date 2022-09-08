namespace APC.Kernel.Models;

public class Artifact {
  public Artifact() {
    Versions = new Dictionary<string, ArtifactVersion>();
  }

  public string Id { get; set; }
  public string Type { get; set; }
  public Dictionary<string, ArtifactVersion> Versions { get; set; }

  public bool AddVersion(ArtifactVersion version) {
    if (Versions.ContainsKey(version.Version)) {
      return false;
    }
    Versions.Add(version.Version, version);
    return true;
  }
  
  public bool HasVersion(string version) {
    return Versions.ContainsKey(version);
  }
}