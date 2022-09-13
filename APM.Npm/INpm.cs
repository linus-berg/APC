using APC.Infrastructure.Models;

namespace APM.Npm;

public interface INpm {
  public Task<Artifact> ProcessArtifact(string name);
}