namespace APM.Jetbrains.Models;

public record JetbrainsPluginUpdate {
  public int id { get; init; }
  public int pluginId { get; init; }
  public string version { get; init; }
  public string file { get; init; }
}