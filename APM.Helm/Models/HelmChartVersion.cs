namespace APM.Helm.Models;

public class HelmChartVersion {
  public required string version { get; set; }
  public bool prerelease { get; set; }
}