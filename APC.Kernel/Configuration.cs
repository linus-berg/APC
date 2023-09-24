using System.Data;
using APC.Kernel.Constants;

namespace APC.Kernel;

public static class Configuration {
  private static readonly Dictionary<ApcVariable, string> DEFAULTS_ = new() {
    {
      ApcVariable.APC_API_HOST, "http://localhost:4001"
    }, {
      ApcVariable.APC_ACM_DIR, "/data/"
    }, {
      ApcVariable.APC_REDIS_HOST, "localhost"
    }, {
      ApcVariable.APC_REDIS_USER, ""
    }, {
      ApcVariable.APC_REDIS_PASS, ""
    }, {
      ApcVariable.APC_RABBIT_MQ_HOST, "localhost"
    }, {
      ApcVariable.APC_RABBIT_MQ_USER, "guest"
    }, {
      ApcVariable.APC_RABBIT_MQ_PASS, "guest"
    }, {
      ApcVariable.APC_OTEL_HOST, "http://localhost:4318"
    },
    {
      ApcVariable.ACM_HTTP_DELTA, "true" // Create daily deltas
    },
    {
      ApcVariable.ACM_HTTP_MODE, "lake" // lake, forward 
    }
  };

  public static string GetApcVar(ApcVariable variable) {
    string? value = Environment.GetEnvironmentVariable(variable.ToString());

    /* If variable is set */
    if (value != null) {
      return value;
    }

    /* If variable has default */
    if (DEFAULTS_.TryGetValue(variable, out string? default_value)) {
      return default_value;
    }

    throw new NoNullAllowedException($"{variable.ToString()} is null!");
  }
}