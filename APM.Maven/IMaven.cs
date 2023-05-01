using MavenNet.Models;
using Artifact = APC.Kernel.Models.Artifact;

namespace APM.Maven;

public interface IMaven {
  public Task<Artifact> ProcessArtifact(Artifact artifact);
  public Task<Metadata> GetMetadata(string g, string id);
  public Task<Project> GetPom(string g, string id, string v);

  public string GetDocsPath(string group, string id, string version,
                            string src_postfix, string src_ext);

  public string GetSrcPath(string group, string id, string version,
                           string src_postfix, string src_ext);

  public string GetLibraryPath(string group, string id, string version,
                               string packaging);

  public string GetPomPath(string group, string id, string version);
}