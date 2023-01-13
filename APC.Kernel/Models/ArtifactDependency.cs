namespace APC.Kernel.Models;

public class ArtifactDependency {
  public string id { get; set; }
  public string processor { get; set; }

  public override bool Equals(object? obj) {
    ArtifactDependency dep = obj as ArtifactDependency;
    return dep != null && id.Equals(dep.id);
  }

  public override int GetHashCode() {
    return id.GetHashCode();
  }
}