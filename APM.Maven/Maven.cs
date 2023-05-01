using System.Xml;
using System.Xml.Serialization;
using APC.Kernel.Exceptions;
using APC.Kernel.Models;
using MavenNet;
using MavenNet.Models;
using Artifact = APC.Kernel.Models.Artifact;
using MavenArtifact = MavenNet.Models.Artifact;

namespace APM.Maven;

public class Maven : IMaven {
  private const string MAVEN_ = "https://repo1.maven.org/maven2";
  private readonly MavenCentralRepository repo_;

  public Maven() {
    repo_ = MavenRepository.FromMavenCentral();
  }

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    string group_id = GetGroup(artifact);
    string artifact_id = artifact.id;
    Metadata metadata = await GetMetadata(group_id, artifact_id);
    if (metadata == null) {
      throw new ArtifactMetadataException();
    }

    foreach (string version in metadata.AllVersions) {
      /* Get pom for version */
      Project project = await GetPom(group_id, artifact_id, version);
      if (project == null) {
        continue;
      }

      /* Add pom and lib to collection */
      artifact.AddVersion(GetPomVersion(group_id, artifact.id, version));
      artifact.AddVersion(
        GetLibraryVersion(group_id, artifact.id, version, project.Packaging));

      await AddDependencies(artifact, project);
    }

    return artifact;
  }

  private async Task AddDependencies(Artifact artifact, Project project) {
    foreach (Dependency dep in project.Dependencies) {
      ArtifactDependency apc_dep =
        artifact.AddDependency(dep.ArtifactId, "maven");
      if (dep.GroupId.Contains("groupId")) {
        apc_dep.config["group"] = project.GroupId;
      } else {
        apc_dep.config["group"] = dep.GroupId;
      }
    }
  }


  public static T Parse<T>(Stream stream, bool ns = true) {
    T result = default;
    XmlSerializer serializer = new(typeof(T));
    using StreamReader sr = new(stream);
    result = (T)serializer.Deserialize(new XmlTextReader(sr) {
      Namespaces = ns
    });
    return result;
  }

  private ArtifactVersion
    GetPomVersion(string group, string id, string version) {
    return new ArtifactVersion {
      location = GetPomPath(group, id, version),
      version = $"{version}-pom"
    };
  }

  private ArtifactVersion
    GetLibraryVersion(string group, string id, string version,
                      string packaging) {
    return new ArtifactVersion {
      location = GetLibraryPath(group, id, version, packaging),
      version = $"{version}-lib"
    };
  }

  public async Task<Metadata> GetMetadata(string g, string id) {
    Metadata m = null;
    try {
      await using Stream s = await repo_.OpenMavenMetadataFile(g, id);
      m = Parse<Metadata>(s);
    } catch {
    }

    return m;
  }

  public async Task<Project> GetPom(string g, string id, string v) {
    Project p = null;
    try {
      await using Stream s = await repo_.OpenArtifactPomFile(g, id, v);
      p = Parse<Project>(s, false);
    } catch {
    }

    return p;
  }

  public string GetPomPath(string group, string id, string version) {
    return Path.Combine(MAVEN_, group, id, version, $"{id}-{version}.pom");
  }

  public string GetDocsPath(string group, string id, string version,
                            string src_postfix, string src_ext) {
    return Path.Combine(MAVEN_, group, id, version,
                        $"{id}-{version}-{src_postfix}.{src_ext.TrimStart('.')}");
  }

  public string GetSrcPath(string group, string id, string version,
                           string src_postfix, string src_ext) {
    return Path.Combine(MAVEN_, group, id, version,
                        $"{id}-{version}-{src_postfix}.{src_ext.TrimStart('.')}");
  }

  public string GetLibraryPath(string group, string id, string version,
                               string packaging) {
    return Path.Combine(MAVEN_, group, id, version,
                        $"{id}-{version}.{packaging.ToLowerInvariant().TrimStart('.')}");
  }

  private string GetGroup(Artifact artifact) {
    if (!artifact.config.ContainsKey("group")) {
      throw new MissingFieldException("Group is missing from artifact");
    }

    return artifact.config["group"].Replace(".", "/");
  }
}