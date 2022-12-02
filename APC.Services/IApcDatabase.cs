using APC.Services.Models;

namespace APC.Services;

public interface IApcDatabase {
  public Task AddArtifact(Artifact artifact);
  public Task<bool> UpdateArtifact(Artifact artifact);
  public Task<IEnumerable<string>> GetModules();
  public Task<Artifact> GetArtifactByName(string name, string module);
  public Task<IEnumerable<Artifact>> GetArtifacts(string module);
  public Task<IEnumerable<Artifact>> GetRoots(string module);
  public Task<bool> DeleteArtifact(Artifact artifact);
}