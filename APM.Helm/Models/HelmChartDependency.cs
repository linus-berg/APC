namespace APM.Helm.Models;

public class HelmChartDependency {
  public required string name { get; set; }
  public required string repository { get; set; }
  public required string artifacthub_repository_name { get; set; }
}