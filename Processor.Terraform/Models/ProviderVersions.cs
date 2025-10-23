namespace Processor.Terraform.Models;

public class ProviderVersions {
  public string id { get; set; }

  public List<ProviderVersion> versions { get; set; }
}