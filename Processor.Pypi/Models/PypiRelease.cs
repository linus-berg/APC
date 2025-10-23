namespace APM.Pypi.Models;

public class PypiRelease {
  public string filename { get; set; }
  public string url { get; set; }
  public string packagetype { get; set; }
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