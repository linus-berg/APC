using APC.Services.Models;

namespace APM.Nuget;

public interface INuget {
  public Task<Artifact> ProcessArtifact(string name);
}