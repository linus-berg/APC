using Dapper.Contrib.Extensions;

namespace APC.Services.Models;

[Table("artifact_dependencies")]
public class ArtifactDependency {
  public int id { get; set; }
  public int artifact_id { get; set; }
  public string name { get; set; }
  public string module { get; set; }

  public override bool Equals(object? obj) {
    ArtifactDependency dep = obj as ArtifactDependency;
    return dep != null && name.Equals(dep.name);
  }

  public override int GetHashCode() {
    return name.GetHashCode();
  }
}