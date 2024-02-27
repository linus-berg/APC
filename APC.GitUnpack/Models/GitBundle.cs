using System.Globalization;

namespace APC.GitUnpack.Models;

public class GitBundle {
  public GitBundle(string filepath, string owner) {
    this.filepath = filepath;
    this.owner = owner;
    Parse();
  }

  public string filepath { get; } = "";
  public string repository { get; private set; }
  public string repository_dir { get; private set; }
  public string owner { get; }

  private void Parse() {
    repository = Path.GetFileName(filepath);
    repository_dir =
      Path.Join(
        Environment.GetEnvironmentVariable("GIT_BUNDLE_REPOS"), owner,
        repository);
  }
}