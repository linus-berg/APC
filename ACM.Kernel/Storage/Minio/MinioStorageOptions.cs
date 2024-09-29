namespace ACM.Kernel.Storage.Minio;

public class MinioStorageOptions {
  public string connection_string { get; set; }
  public bool auto_create_bucket { get; set; }
}