namespace APC.Skopeo.Models;

public class Image {
  // Input is in form of docker://docker.io/library/nginx:1.2.3
  public Image(string image) {
    Uri = image;
    Repository = GetRepository();
    Destination = GetDestination();
  }

  public string Uri { get; }
  public string Repository { get; }

  public string Destination { get; }

  private string GetDestination() {
    int index = Repository.LastIndexOf(':');
    return index != -1 ? Repository.Remove(index) : Repository;
  }

  private string GetRepository() {
    Uri uri = new(Uri);
    return uri.GetComponents(UriComponents.Host | UriComponents.Path,
                             UriFormat.Unescaped);
  }
}