using Dapper.Contrib.Extensions;

namespace APC.Services.Models;

[Table("artifact_dependencies")]
public class ArtifactDependency {
  public int id { get; set; }
  public int artifact_id { get; set; }
  public string name { get; set; }
}