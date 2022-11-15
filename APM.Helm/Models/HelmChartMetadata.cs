namespace APM.Helm.Models;

public class HelmChartMetadata {
  public HelmChartMetadata() {
    available_versions = new HashSet<HelmChartVersion>();
    containers_images = new HashSet<HelmChartContainerImage>();
  }

  public string name { get; set; }
  public string package_id { get; set; }
  public string content_url { get; set; }
  public string version { get; set; }
  public HelmChartRepository repository { get; set; }
  public HelmChartData data { get; set; }
  public IEnumerable<HelmChartVersion> available_versions { get; set; }
  public IEnumerable<HelmChartContainerImage> containers_images { get; set; }
}