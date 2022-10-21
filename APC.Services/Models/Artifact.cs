using Dapper.Contrib.Extensions;

namespace APC.Services.Models;

[Table("artifacts")]
public class Artifact {
  public Artifact() {
    versions = new Dictionary<string, ArtifactVersion>();
    dependencies = new HashSet<ArtifactDependency>();
  }

  public int id { get; set; }
  public string name { get; set; }
  public string module { get; set; }
  public ArtifactStatus status { get; set; } = ArtifactStatus.PROCESSING;
  public bool root { get; set; } = false;

  [Computed] public Dictionary<string, ArtifactVersion> versions { get; set; }

  [Computed] public HashSet<ArtifactDependency> dependencies { get; set; }

  public bool AddDependency(string name, string module) {
    return dependencies.Add(new ArtifactDependency() {
      name = name,
      module = module
    });
  }

  public HashSet<string> VersionDiff(HashSet<ArtifactVersion> versions_in_db) {
    HashSet<string> diff = new();
    foreach (KeyValuePair<string, ArtifactVersion> version in versions)
      if (versions_in_db.All(v => v.version != version.Key))
        diff.Add(version.Key);
    return diff;
  }

  public bool AddVersion(ArtifactVersion version) {
    if (versions.ContainsKey(version.version)) return false;
    versions.Add(version.version, version);
    return true;
  }

  public bool HasVersion(string version) {
    return versions.ContainsKey(version);
  }
}