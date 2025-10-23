using MavenNet.Models;
using Artifact = Core.Kernel.Models.Artifact;

namespace APM.Maven;

public interface IMaven {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
  public Task<Metadata> GetMetadata(string g, string id);

  public Task<Dictionary<string, List<string>>>
    SearchMaven(string g, string id);

  public Task<Project> GetPom(string g, string id, string v);
}