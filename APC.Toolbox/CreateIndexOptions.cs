using CommandLine;

namespace APC.Toolbox;

[Verb("create-index")]
public class CreateIndexOptions : IOptions {
  [Option("processor", Required = true)]
  public string Processor { get; set; } = null!;
}