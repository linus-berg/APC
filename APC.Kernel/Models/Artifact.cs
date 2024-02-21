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
  public ArtifactFilterType filterType { get; set; }
  public ArtifactStatus status { get; set; } = ArtifactStatus.PROCESSING;
  public bool root { get; set; } = false;

  public Dictionary<string, ArtifactVersion> versions { get; set; }
  public Dictionary<string, string> config { get; set; }

  public HashSet<ArtifactDependency> dependencies { get; set; }

  public ArtifactDependency AddDependency(string id, string processor) {
    ArtifactDependency dep = new() {
      id = id,
      processor = processor
    };
    dependencies.Add(dep);
    return dep;
  }

  public bool AddVersion(ArtifactVersion version) {
    return versions.TryAdd(version.version, version);
  }

  public bool HasVersion(string version) {
    return versions.ContainsKey(version);
  }
}
