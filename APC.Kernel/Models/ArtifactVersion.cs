using System.Collections.Generic;

namespace APC.Kernel.Models;

public class ArtifactVersion {
  public string version { get; set; } = "-";

  public ArtifactVersionStatus status { get; set; } =
    ArtifactVersionStatus.SENT_FOR_COLLECTION;

  public Dictionary<string, ArtifactFile> files { get; set; } = new();

  public void AddFile(string name, string uri, string folder = "") {
    files[name] = new ArtifactFile {
      uri = uri,
      folder = folder
    };
  }
}