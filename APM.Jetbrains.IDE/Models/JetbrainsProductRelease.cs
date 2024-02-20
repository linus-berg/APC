namespace APM.Jetbrains.IDE.Models;

public class JetbrainsProductRelease {
  public required string version { get; set; }

  public required Dictionary<string, JetbrainsProductDownload> downloads {
    get;
    set;
  }
}