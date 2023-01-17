using CliWrap;

namespace ACM.Git;

public class Git {
  private readonly string dir_;

  public Git(string dir) {
    dir_ = dir;
  }

  public async Task<bool> Mirror(string repo) {
    string repository = GetFullRepoPath(repo);
    if (Directory.Exists(repository)) {
      return await Update(repository);
    }

    UriBuilder uri = new(repo);
    uri.Scheme = Uri.UriSchemeHttps;
    uri.Port = -1;
    Command cmd =
      GetCommand(GetRepoPath(uri.Uri), $"clone --mirror {uri.Uri}");
    bool failed = false;
    try {
      CommandResult result = await cmd.ExecuteAsync();
      failed = result.ExitCode == 0;
    } catch (Exception e) {
      Console.WriteLine(e);
      throw;
    }

    return failed;
  }

  public async Task<bool> Update(string repo) {
    Command cmd = GetCommand(repo, "remote update");
    CommandResult result = await cmd.ExecuteAsync();
    return result.ExitCode == 0;
  }

  private Command GetCommand(string wd, string args) {
    Directory.CreateDirectory(wd);
    return Cli.Wrap("git")
              .WithArguments(args)
              .WithWorkingDirectory(wd);
  }

  private string GetFullRepoPath(string repo) {
    string repo_path = GetRepoPath(new Uri(repo));
    string path = Path.Join(repo_path, Path.GetFileName(repo));
    return path;
  }

  private string GetRepoPath(Uri uri) {
    string host = uri.Host;
    string path = Path.GetDirectoryName(uri.LocalPath);
    return Path.Join(dir_, host, path);
  }
}