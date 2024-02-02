using System.Collections.Generic;

namespace APC.Kernel.Models;

public class ArtifactDependency {
  public string id { get; set; }
  public string processor { get; set; }

  public Dictionary<string, string> config { get; set; } = new();

  public override bool Equals(object? obj) {
    ArtifactDependency dep = obj as ArtifactDependency;
    return dep != null && id.Equals(dep.id);
  }

  public override int GetHashCode() {
    return id.GetHashCode();
  }
}