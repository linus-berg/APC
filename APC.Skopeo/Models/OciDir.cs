namespace APC.Skopeo.Models;

public class OciDir {
  public OciDir(string directory) {
    Directory = directory;
    Shared = Path.Join(Directory, "shared");
    Repositories = Path.Join(Directory, "repositories");
  }

  public string Repositories { get; }

  public string Shared { get; }

  public string Directory { get; }

  public string GetImageRoot(Image image) {
    string path = image.Repository;
    path = path.Remove(path.LastIndexOf("/"));
    return Path.Join(Repositories, path);
  }
}