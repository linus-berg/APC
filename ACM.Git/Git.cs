using APC.Kernel.Extensions;
using CliWrap;

namespace ACM.Git;

public class Git {
  private readonly string dir_;

  public Git(string dir) {
    dir_ = dir;
    ConfigureProxy().Wait();
  }

  private async Task<bool> ConfigureProxy() {
    Command cmd = Cli.Wrap("git")
                     .WithArguments(
                       $"config --global http.proxy {Environment.GetEnvironmentVariable("HTTPS_PROXY")}");
    return await cmd.ExecuteToConsole();
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
    return await cmd.ExecuteToConsole();
  }

  public async Task<bool> Update(string repo) {
    Command cmd = GetCommand(repo, "remote update");
    return await cmd.ExecuteToConsole();
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