namespace APM.Helm.Models;

public class HelmChartDependency {
  public string name { get; set; }
  public string repository { get; set; }
  public string artifacthub_repository_name { get; set; }
}