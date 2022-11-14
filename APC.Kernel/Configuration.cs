using System.Data;

namespace APC.Kernel;

public static class Configuration {
  private static readonly Dictionary<ApcVariable, string> DEFAULTS_ = new() {
    { ApcVariable.APC_API_HOST, "http://localhost:4001" },
    { ApcVariable.APC_ACM_DIR, "/data/" },

    { ApcVariable.APC_REDIS_HOST, "localhost" },
    { ApcVariable.APC_REDIS_USER, "" },
    { ApcVariable.APC_REDIS_PASS, "" },

    { ApcVariable.APC_RABBIT_MQ_HOST, "localhost" },
    { ApcVariable.APC_RABBIT_MQ_USER, "guest" },
    { ApcVariable.APC_RABBIT_MQ_PASS, "guest" }
  };

  public static string GetApcVar(ApcVariable variable) {
    string value = Environment.GetEnvironmentVariable(variable.ToString());

    /* If variable is set */
    if (value != null) return value;

    /* If variable has default */
    if (DEFAULTS_.ContainsKey(variable)) return DEFAULTS_[variable];
    throw new NoNullAllowedException($"{variable.ToString()} is null!");
  }
}