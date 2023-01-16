namespace APC.Kernel.Models;

public class Artifact {
  public Artifact() {
    versions = new Dictionary<string, ArtifactVersion>();
    dependencies = new HashSet<ArtifactDependency>();
    config = new Dictionary<string, string>();
  }

  public string id { get; set; }
  public string processor { get; set; }
  public string filter { get; set; }

  public ArtifactStatus status { get; set; } = ArtifactStatus.PROCESSING;
  public bool root { get; set; } = false;

  public Dictionary<string, ArtifactVersion> versions { get; set; }
  public Dictionary<string, string> config { get; set; }

  public HashSet<ArtifactDependency> dependencies { get; set; }

  public bool AddDependency(string id, string processor) {
    return dependencies.Add(new ArtifactDependency {
      id = id,
      processor = processor
    });
  }

  public bool AddVersion(ArtifactVersion version) {
    if (versions.ContainsKey(version.version)) {
      return false;
    }

    versions.Add(version.version, version);
    return true;
  }

  public bool HasVersion(string version) {
    return versions.ContainsKey(version);
  }
}