namespace APC.Kernel.Models;

public class ArtifactVersion {
  public string version { get; set; } = "-";

  public ArtifactVersionStatus status { get; set; } =
    ArtifactVersionStatus.SENT_FOR_COLLECTION;

  public Dictionary<string, ArtifactFile> files { get; set; } = new Dictionary<string, ArtifactFile>();

  public void AddFile(string name, string uri, string module) {
    files[name] = new ArtifactFile() {
      uri = uri,
      processor = module
    };
  }
}