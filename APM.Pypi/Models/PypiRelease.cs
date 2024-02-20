namespace APM.Pypi.Models;

public class PypiRelease {
  public required string filename { get; set; }
  public required string url { get; set; }
  public required string packagetype { get; set; }
  public bool yanked { get; set; }

  public bool IsValid() {
    if (filename.Contains("macos")) {
      return false;
    }

    if (yanked) {
      return false;
    }

    return packagetype != "bdist_wininst";
  }
}