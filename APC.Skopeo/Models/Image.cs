namespace APC.Skopeo.Models;

public class Image {
  // Input is in form of docker://docker.io/library/nginx:1.2.3
  public Image(string image) {
    Uri = image;
    Reference = GetImageRef();
    Repository = GetRepository();
    Name = GetName();
  }

  public string Name { get; }
  public string Repository { get; }
  public string Reference { get; }
  public string Uri { get; }

  private string GetRepository() {
    string repo = Reference;
    if (Reference.Contains(":")) {
      repo = Reference.Remove(Reference.IndexOf(":"));
    }

    return repo;
  }

  private string GetName() {
    return Repository.Substring(Repository.LastIndexOf("/") + 1);
  }

  private string GetImageRef() {
    Uri uri = new(Uri);
    return uri.GetComponents(UriComponents.Host | UriComponents.Path,
                             UriFormat.Unescaped);
  }
}