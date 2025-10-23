using CommandLine;

namespace Backpack.Toolbox;

[Verb("create-index")]
public class CreateIndexOptions : IOptions {
  [Option("processor", Required = true)]
  public string processor { get; set; } = null!;
}