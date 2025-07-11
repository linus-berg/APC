namespace APC.Skopeo.Models;

public class SkopeoArchive {
  private readonly Uri uri_;
  private readonly string target_;
  private readonly string tar_name_;
  private readonly string tar_path_;
  public SkopeoArchive(string remote_image, string target_dir) {
    uri_ = new Uri(remote_image);
    target_ = $"{uri_.Host}{uri_.PathAndQuery}";
    tar_name_ = target_.Replace("/", "_").Replace(":", "_").Replace(".", "_");
    tar_path_ = Path.Join(target_dir, $"{tar_name_}.tar");
  }

  public Uri Uri => uri_;
  public string Target => target_;
  public string TarPath => tar_path_;
  public string TarName => tar_name_;
  public string TarWithHost => $"{Path.Join(uri_.Host, tar_name_)}.tar";
}