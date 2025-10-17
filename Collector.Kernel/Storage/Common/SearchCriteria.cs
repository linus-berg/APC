using System.Text.RegularExpressions;

namespace Collector.Kernel.Storage.Common;

public class SearchCriteria {
  public string prefix { get; set; }
  public Regex pattern { get; set; }
}