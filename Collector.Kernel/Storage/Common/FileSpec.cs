namespace Collector.Kernel.Storage.Common;

public class FileSpec {
  public string path { get; set; }
  public DateTime created { get; set; }
  public DateTime modified { get; set; }

  /// <summary>
  ///   In Bytes
  /// </summary>
  public long size { get; set; }
}