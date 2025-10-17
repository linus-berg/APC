namespace Collector.Git;

public class Repository {
  private readonly string original_uri_;
  private readonly UriBuilder uri_;


  public Repository(string repo, string local_directory) {
    original_uri_ = repo;
    uri_ = new UriBuilder(original_uri_) {
      Scheme = Uri.UriSchemeHttps,
      Port = -1
    };
    owner = GetOwner();
    remote = uri_.Uri.ToString();

    name = GetName();
    directory = GetDirectory();
    local_path = Path.Join(local_directory, directory);
  }

  public string name { get; }

  public string owner { get; }

  public string remote { get; }

  public string directory { get; }

  public string local_path { get; }

  private string GetName() {
    string filename = Path.GetFileName(original_uri_);
    if (original_uri_.EndsWith(".git")) {
      return filename.Substring(0, filename.Length - 4);
    }

    return filename;
  }

  private string GetDirectory() {
    return Path.Join(owner, name);
  }

  private string GetOwner() {
    string host = uri_.Host;
    string path = Path.GetDirectoryName(uri_.Uri.LocalPath);
    return Path.Join(host, path);
  }
}