using Dapper.Contrib.Extensions;

namespace APC.Infrastructure.Models;

[Table("artifact_versions")]
public class ArtifactVersion {
  public ArtifactVersion() {
    dependencies = new HashSet<string>();
  }
  public int id { get; set; }
  public int artifact_id { get; set; }
  public string version { get; set; }
  public ArtifactVersionStatus status { get; set; } = ArtifactVersionStatus.SENT_FOR_COLLECTION;
  public string location { get; set; }
  
  [Computed] public HashSet<string> dependencies { get; set; }
  public bool AddDependency(string id) {
    return dependencies.Add(id);
  }
}