using System.Data;

namespace APC.Kernel; 

public static class Configuration {
  public enum APC_VAR {
    APC_RABBIT_MQ_HOST,
    APC_RABBIT_MQ_USER,
    APC_RABBIT_MQ_PASS,
    
    APC_REDIS_HOST,
    APC_REDIS_USER,
    APC_REDIS_PASS,
    
    APC_API_HOST,
  }

  public static string GetAPCVar(APC_VAR var) {
    string value = Environment.GetEnvironmentVariable(var.ToString());
    if (value == null) {
      throw new NoNullAllowedException($"{var.ToString()} is null!");
    }
    return value;
  }

}