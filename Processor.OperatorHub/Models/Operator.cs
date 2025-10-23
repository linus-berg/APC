namespace Processor.OperatorHub.Models;

public class Operator {
  public string name { get; set; }
  public string version { get; set; }
  public string containerImage { get; set; }
  public string channel { get; set; }
  public string packageName { get; set; }

  public List<OperatorChannel> channels { get; set; }
}