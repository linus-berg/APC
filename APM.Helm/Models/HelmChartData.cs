namespace APM.Helm.Models;

public class HelmChartData {
  public required string version { get; set; }
  public bool prerelease { get; set; }
  public required IEnumerable<HelmChartDependency> dependencies { get; set; }
}