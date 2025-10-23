using System.Data;

namespace Core.Kernel;

public static class Configuration {
  private static readonly Dictionary<CoreVariables, string> S_DEFAULTS_ = new() {
    {
      CoreVariables.APC_API_HOST, "http://localhost:4001"
    }, {
      CoreVariables.APC_ACM_DIR, "/data/"
    }, {
      CoreVariables.APC_REDIS_HOST, "localhost"
    }, {
      CoreVariables.APC_REDIS_USER, ""
    }, {
      CoreVariables.APC_REDIS_PASS, ""
    }, {
      CoreVariables.APC_RABBIT_MQ_HOST, "localhost"
    }, {
      CoreVariables.APC_RABBIT_MQ_USER, "guest"
    }, {
      CoreVariables.APC_RABBIT_MQ_PASS, "guest"
    }, {
      CoreVariables.APC_OTEL_HOST, "http://localhost:4318"
    }, {
      CoreVariables.ACM_HTTP_DELTA, "true" // Create daily deltas
    }, {
      CoreVariables.ACM_HTTP_MODE, "lake" // lake, forward 
    }
  };

  public static bool HasOtelHost() {
    bool has_otel_host = false;
    try {
      has_otel_host =
        !string.IsNullOrEmpty(GetApcVar(CoreVariables.APC_OTEL_HOST));
    } catch {
      has_otel_host = false;
    }

    return has_otel_host;
  }

  public static string? GetApcVar(CoreVariables variable) {
    string? value = Environment.GetEnvironmentVariable(variable.ToString());

    /* If variable is set */
    if (value != null) {
      return value;
    }

    /* If variable has default */
    if (S_DEFAULTS_.TryGetValue(variable, out string? default_value)) {
      return default_value;
    }

    throw new NoNullAllowedException($"{variable.ToString()} is null!");
  }
}