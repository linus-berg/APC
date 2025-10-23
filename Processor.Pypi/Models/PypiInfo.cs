namespace Processor.Pypi.Models;

public class PypiInfo {
  public List<string>? requires_dist { get; set; }

  public List<string> GetDependencies() {
    List<string> dependencies = new();
    if (requires_dist == null) {
      return dependencies;
    }

    foreach (string dist in requires_dist) {
      string dependency = dist.Split(' ')[0];
      if (dependency.Contains("=") ||
          dependency.Contains("<") ||
          dependency.Contains(">") ||
          dependency.Contains(";")) {
        continue;
      }

      dependencies.Add(dist.Split(' ')[0]);
    }

    return dependencies;
  }
}