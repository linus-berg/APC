using CommandLine;

namespace APC.Toolbox;

[Verb("create-data")]
public class DataGeneratorOptions {
  [Option("processor", Required = true)]
  public string processor { get; set; } = null!;

  [Option("files", Required = true)]
  public int files { get; set; }
}