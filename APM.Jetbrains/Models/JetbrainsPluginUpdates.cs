namespace APM.Jetbrains.Models;

public record JetbrainsPluginUpdate {
  public int id { get; init; }
  public int pluginId { get; init; }
  public required string version { get; init; }
  public required string file { get; init; }
}