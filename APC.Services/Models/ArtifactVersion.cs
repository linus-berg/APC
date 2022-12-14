using Dapper.Contrib.Extensions;

namespace APC.Services.Models;

[Table("artifact_versions")]
public class ArtifactVersion {
  public int id { get; set; }
  public int artifact_id { get; set; }
  public string version { get; set; } = "-";

  public ArtifactVersionStatus status { get; set; } =
    ArtifactVersionStatus.SENT_FOR_COLLECTION;

  public string location { get; set; }
}