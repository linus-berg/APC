namespace APM.Pypi.Models;

public class PypiMetadata {
  public required PypiInfo info { get; set; }
  public required Dictionary<string, List<PypiRelease>> releases { get; set; }

  public Dictionary<string, List<PypiRelease>> GetAllValidReleases() {
    Dictionary<string, List<PypiRelease>> valid = new();

    foreach (KeyValuePair<string, List<PypiRelease>> kv in releases) {
      List<PypiRelease> releases = kv.Value;
      string version = kv.Key;
      foreach (PypiRelease release in releases) {
        if (!release.IsValid()) {
          continue;
        }

        if (valid.ContainsKey(version)) {
          valid[version].Add(release);
        } else {
          valid[version] = new List<PypiRelease>();
          valid[version].Add(release);
        }
      }
    }

    return valid;
  }
}