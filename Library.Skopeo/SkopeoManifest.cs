namespace Library.Skopeo;

public class SkopeoManifest {
  private string working_dir_;
  public string Digest { get; set; }
  public string Created { get; set; }
  public string Os { get; set; }
  public string Architecture { get; set; }
  public List<string> Layers { get; set; } = new();

  public string WorkingDirectory {
    get => working_dir_;
    set {
      working_dir_ = value;
      layer_dir_ = Path.Join(value, "shared", "sha256");
    }
  }

  private string layer_dir_ { get; set; }

  public bool VerifyLayers() {
    bool complete = true;
    foreach (string layer in Layers) {
      if (!File.Exists(GetLayerPath(layer))) {
        return false;
      }
    }

    return complete;
  }

  private string GetLayerPath(string layer) {
    return Path.Join(layer_dir_, GetLayerName(layer));
  }

  private string GetLayerName(string layer) {
    return layer.Replace("sha256:", "");
  }
}