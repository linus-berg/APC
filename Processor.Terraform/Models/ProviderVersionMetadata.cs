namespace Processor.Terraform.Models;

public class ProviderVersionMetadata {
  public string download_url { get; set; }
  public string shasums_url { get; set; }
  public string shasums_signature_url { get; set; }
}