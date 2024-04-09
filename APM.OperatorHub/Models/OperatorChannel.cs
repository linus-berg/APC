namespace APM.OperatorHub.Models;

public class OperatorChannel {
  public string name { get; set; }
  public List<OperatorVersion> versions { get; set; }
}