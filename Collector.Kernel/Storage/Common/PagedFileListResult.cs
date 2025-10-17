using System.Collections.ObjectModel;

namespace Collector.Kernel.Storage.Common;

public interface IHasNextPageFunc {
  Func<PagedFileListResult, Task<NextPageResult>> next_page_func { get; set; }
}

public class PagedFileListResult : IHasNextPageFunc {
  private static readonly IReadOnlyCollection<FileSpec> S_EMPTY_ =
    new ReadOnlyCollection<FileSpec>(Array.Empty<FileSpec>());

  public static readonly PagedFileListResult S_EMPTY = new(S_EMPTY_);

  public PagedFileListResult(IReadOnlyCollection<FileSpec> files) {
    this.files = files;
    has_more = false;
    ((IHasNextPageFunc)this).next_page_func = null;
  }

  public PagedFileListResult(IReadOnlyCollection<FileSpec> files, bool has_more,
                             Func<PagedFileListResult, Task<NextPageResult>>
                               next_page_func) {
    this.files = files;
    this.has_more = has_more;
    ((IHasNextPageFunc)this).next_page_func = next_page_func;
  }

  public PagedFileListResult(
    Func<PagedFileListResult, Task<NextPageResult>> next_page_func) {
    ((IHasNextPageFunc)this).next_page_func = next_page_func;
  }

  public IReadOnlyCollection<FileSpec> files { get; private set; }
  public bool has_more { get; private set; }

  protected IDictionary<string, object> data { get; } =
    new Dictionary<string, object>();

  Func<PagedFileListResult, Task<NextPageResult>> IHasNextPageFunc.
    next_page_func { get; set; }

  public async Task<bool> NextPageAsync() {
    if (((IHasNextPageFunc)this).next_page_func == null) {
      return false;
    }

    NextPageResult result = await ((IHasNextPageFunc)this).next_page_func(this);
    if (result.success) {
      files = result.files;
      has_more = result.has_more;
      ((IHasNextPageFunc)this).next_page_func = result.next_page_func;
    } else {
      files = S_EMPTY_;
      has_more = false;
      ((IHasNextPageFunc)this).next_page_func = null;
    }

    return result.success;
  }
}