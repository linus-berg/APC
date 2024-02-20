using System.Globalization;

namespace APC.GitUnpack.Models;

public class GitBundle {
  public GitBundle(string filepath, string owner) {
    this.filepath = filepath;
    this.owner = owner;
    Parse();
  }

  public bool is_first_bundle { get; private set; }

  public string filepath { get; } = "";
  public string repository { get; private set; }
  public string repository_dir { get; private set; }

  public DateTime from { get; private set; }
  public DateTime to { get; private set; }
  public string owner { get; }

  private void Parse() {
    string[] parts = Path.GetFileNameWithoutExtension(filepath).Split("@");
    repository = parts[0];
    string[] timeline = parts[1].Split("-");
    DateTime.TryParseExact(timeline[0], "yyyyMMddHHmmss", null,
                           DateTimeStyles.None, out DateTime from);
    this.from = from;

    DateTime.TryParseExact(timeline[1], "yyyyMMddHHmmss", null,
                           DateTimeStyles.None, out DateTime to);
    this.to = to;

    is_first_bundle = this.from == DateTime.UnixEpoch;
    repository_dir =
      Path.Join(
        Environment.GetEnvironmentVariable("GIT_BUNDLE_REPOS"), owner,
        repository);
  }

  public string MoveToApply() {
    string tmp_file = Path.Join(Path.GetDirectoryName(filepath), repository);
    File.Move(filepath, tmp_file);
    return tmp_file;
  }
}