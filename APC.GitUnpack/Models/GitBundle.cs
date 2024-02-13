using System.Globalization;

namespace APC.GitUnpack.Models;

public class GitBundle {
  public GitBundle(string filepath, string owner) {
    Filepath = filepath;
    Owner = owner;
    Parse();
  }

  public bool IsFirstBundle { get; private set; }

  public string Filepath { get; } = "";
  public string Repository { get; private set; }
  public string RepositoryDir { get; private set; }

  public DateTime From { get; private set; }
  public DateTime To { get; private set; }
  public string Owner { get; private set; }

  private void Parse() {
    string[] parts = Path.GetFileNameWithoutExtension(Filepath).Split("@");
    Repository = parts[0];
    string[] timeline = parts[1].Split("-");
    DateTime.TryParseExact(timeline[0], "yyyyMMddHHmmss", null,
                           DateTimeStyles.None, out DateTime from);
    From = from;

    DateTime.TryParseExact(timeline[1], "yyyyMMddHHmmss", null,
                           DateTimeStyles.None, out DateTime to);
    To = to;

    IsFirstBundle = From == DateTime.UnixEpoch;
    RepositoryDir =
      Path.Join(
        Environment.GetEnvironmentVariable("GIT_BUNDLE_REPOS"), Owner, Repository);
  }

  public string MoveToApply() {
    string tmp_file = Path.Join(Path.GetDirectoryName(Filepath), Repository);
    File.Move(Filepath, tmp_file);
    return tmp_file;
  }
}