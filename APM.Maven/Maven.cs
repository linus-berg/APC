using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
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

      Dictionary<string, string> properties = GetProperties(project);
      /* Add pom and lib to collection */
      ArtifactVersion artifact_version = new ArtifactVersion() {
        version = version
      };
      artifact_version.AddFile($"{artifact.id}-{version}-pom",
                               GetPomPath(group_id, artifact.id, version));
      artifact_version.AddFile($"{artifact.id}-{version}-lib",
                               GetLibraryPath(group_id, artifact.id, version,
                                              project.Packaging));

      await AddDependencies(artifact, project, properties);
      await AddPlugins(artifact, project, properties);
    }

    return artifact;
  }

  public async Task<Metadata> GetMetadata(string g, string id) {
    Metadata m = null;
    try {
      await using Stream s = await repo_.OpenMavenMetadataFile(g, id);
      m = Parse<Metadata>(s);
    } catch (Exception e) {
      Console.WriteLine(e);
    }

    return m;
  }

  public async Task<Project> GetPom(string g, string id, string v) {
    Project p = null;
    try {
      await using Stream s = await repo_.OpenArtifactPomFile(g, id, v);
      p = Parse<Project>(s, false);
    } catch (Exception e) {
      Console.WriteLine(e);
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

  private async Task AddDependencies(Artifact artifact, Project project,
                                     Dictionary<string, string> properties) {
    foreach (Dependency dep in project.Dependencies) {
      string id = ReplaceProperties(dep.ArtifactId, properties);
      ArtifactDependency apc_dep =
        artifact.AddDependency(id, "maven");
      if (dep.GroupId.Contains("${project.groupId}")) {
        apc_dep.config["group"] = project.GroupId ?? project.Parent.GroupId;
      } else {
        string gid = ReplaceProperties(dep.GroupId, properties);
        apc_dep.config["group"] = gid;
      }
    }
  }


  private async Task AddPlugins(Artifact artifact, Project project,
                                Dictionary<string, string> properties) {
    if (project.Build is not { PluginsSpecified: true }) {
      return;
    }

    foreach (Plugin plugin in project.Build.Plugins) {
      string id = ReplaceProperties(plugin.ArtifactId, properties);
      ArtifactDependency apc_dep =
        artifact.AddDependency(id, "maven");
      if (plugin.GroupId.Contains("${project.groupId}")) {
        apc_dep.config["group"] = project.GroupId ?? project.Parent.GroupId;
      } else {
        string gid = ReplaceProperties(plugin.GroupId, properties);
        apc_dep.config["group"] = gid;
      }
    }
  }

  private Dictionary<string, string> GetProperties(Project project) {
    Dictionary<string, string> properties = new();
    if (project.Properties == null) {
      return properties;
    }

    foreach (XElement? property in project.Properties.Any) {
      properties[property.Name.LocalName] = property.Value;
    }

    return properties;
  }

  private string ReplaceProperties(string input,
                                   Dictionary<string, string> properties) {
    string v = input;
    try {
      v = Regex.Replace(input, @"\$\{(.+?)\}",
                        m => properties[m.Groups[1].Value]);
    } catch {
      v = input;
    }

    return v;
  }


  private static T Parse<T>(Stream stream, bool ns = true) {
    T result = default;
    XmlSerializer serializer = new(typeof(T));
    using StreamReader sr = new(stream);
    result = (T)serializer.Deserialize(new XmlTextReader(sr) {
      Namespaces = ns
    });
    return result;
  }

  private string GetGroup(Artifact artifact) {
    if (!artifact.config.ContainsKey("group")) {
      throw new MissingFieldException("Group is missing from artifact");
    }

    return artifact.config["group"].Replace(".", "/");
  }
}