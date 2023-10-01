namespace ACM.Git;

public class Repository {
  private readonly string original_uri_;
  private readonly UriBuilder uri_;


  public Repository(string repo, string local_directory) {
    original_uri_ = repo;
    uri_ = new UriBuilder(original_uri_) {
      Scheme = Uri.UriSchemeHttps,
      Port = -1
    };
    Owner = GetOwner();
    Remote = uri_.Uri.ToString();

    Name = GetName();
    Directory = GetDirectory();
    LocalPath = Path.Join(local_directory, Directory);
  }

  public string Name { get; }

  public string Owner { get; }

  public string Remote { get; }

  public string Directory { get; }

  public string LocalPath { get; }

  private string GetName() {
    string name = Path.GetFileName(original_uri_);
    if (original_uri_.EndsWith(".git")) {
      return name.Substring(0, name.Length - 4);
    }

    return name;
  }

  private string GetDirectory() {
    return Path.Join(Owner, Name);
  }

  private string GetOwner() {
    string host = uri_.Host;
    string path = Path.GetDirectoryName(uri_.Uri.LocalPath);
    return Path.Join(host, path);
  }
}