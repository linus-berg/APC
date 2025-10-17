namespace Collector.Kernel.Storage.Common;

public class NextPageResult {
  public bool success { get; set; }
  public bool has_more { get; set; }
  public IReadOnlyCollection<FileSpec> files { get; set; }

  public Func<PagedFileListResult, Task<NextPageResult>> next_page_func {
    get;
    set;
  }
}