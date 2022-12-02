using Dapper.Contrib.Extensions;

namespace APC.Services.Models;

[Table("artifact_versions")]
public class ArtifactVersion {
  public string version { get; set; } = "-";

  public ArtifactVersionStatus status { get; set; } =
    ArtifactVersionStatus.SENT_FOR_COLLECTION;

  public string location { get; set; }
}