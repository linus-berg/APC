namespace Collector.Kernel.Storage.Minio;

public class MinioConnectionBuilder {
  public MinioConnectionBuilder() {
  }

  public MinioConnectionBuilder(string connection_string) {
    if (string.IsNullOrEmpty(connection_string)) {
      throw new ArgumentNullException(nameof(connection_string));
    }

    Parse(connection_string);
  }

  public string access_key { get; set; }

  public string secret_key { get; set; }

  public string region { get; set; }

  public string end_point { get; set; }
  public string bucket { get; set; }

  private void Parse(string connection_string) {
    foreach (string[] option in connection_string
                                .Split(
                                  new[] {
                                    ';'
                                  },
                                  StringSplitOptions.RemoveEmptyEntries
                                )
                                .Where(kvp => kvp.Contains('='))
                                .Select(
                                  kvp => kvp.Split(
                                    new[] {
                                      '='
                                    },
                                    2
                                  )
                                )) {
      string option_key = option[0].Trim();
      string option_value = option[1].Trim();
      if (!ParseItem(option_key, option_value)) {
        throw new ArgumentException(
          $"The option '{option_key}' cannot be recognized in connection string.",
          nameof(connection_string)
        );
      }
    }
  }

  protected virtual bool ParseItem(string key, string value) {
    if (string.Equals(key, "AccessKey", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(key, "Access Key", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(key, "AccessKeyId", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(
          key,
          "Access Key Id",
          StringComparison.OrdinalIgnoreCase
        ) ||
        string.Equals(key, "Id", StringComparison.OrdinalIgnoreCase)) {
      access_key = value;
      return true;
    }

    if (string.Equals(key, "SecretKey", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(key, "Secret Key", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(
          key,
          "SecretAccessKey",
          StringComparison.OrdinalIgnoreCase
        ) ||
        string.Equals(
          key,
          "Secret Access Key",
          StringComparison.OrdinalIgnoreCase
        ) ||
        string.Equals(key, "Secret", StringComparison.OrdinalIgnoreCase)) {
      secret_key = value;
      return true;
    }

    if (string.Equals(key, "Region", StringComparison.OrdinalIgnoreCase)) {
      region = value;
      return true;
    }

    if (string.Equals(key, "EndPoint", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(key, "End Point", StringComparison.OrdinalIgnoreCase)) {
      end_point = value;
      return true;
    }

    if (string.Equals(key, "Bucket", StringComparison.OrdinalIgnoreCase)) {
      bucket = value;
      return true;
    }

    return false;
  }

  public override string ToString() {
    string connection_string = string.Empty;
    if (!string.IsNullOrEmpty(access_key)) {
      connection_string += "AccessKey=" + access_key + ";";
    }

    if (!string.IsNullOrEmpty(secret_key)) {
      connection_string += "SecretKey=" + secret_key + ";";
    }

    if (!string.IsNullOrEmpty(region)) {
      connection_string += "Region=" + region + ";";
    }

    if (!string.IsNullOrEmpty(end_point)) {
      connection_string += "EndPoint=" + end_point + ";";
    }

    if (!string.IsNullOrEmpty(bucket)) {
      connection_string += "Bucket=" + bucket + ";";
    }

    return connection_string;
  }
}