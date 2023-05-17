namespace APM.Pypi.Models;

public class PypiInfo {
  public List<string> requires_dist { get; set; }

  public List<string> GetDependencies() {
    List<string> dependencies = new();
    foreach (string dist in requires_dist) {
      dependencies.Add(dist.Split(' ')[0]);
    }

    return dependencies;
  }
}