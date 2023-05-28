namespace APC.Skopeo.Models;

public class Image {
  // Input is in form of docker://docker.io/library/nginx:1.2.3
  public Image(string image) {
    Uri = image;
    Repository = GetRepository();
  }

  public string Uri { get; }
  public string Repository { get; }

  private string GetRepository() {
    Uri uri = new(Uri);
    return uri.GetComponents(UriComponents.Host | UriComponents.Path,
                             UriFormat.Unescaped);
  }
}