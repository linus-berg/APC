namespace APM.Jetbrains.IDE.Models; 

public class JetbrainsProductRelease {
  public string version { get; set; }
  public Dictionary<string, JetbrainsProductDownload> downloads { get; set; }
}