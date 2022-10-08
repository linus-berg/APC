using APC.Infrastructure.Models;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace APM.Nuget; 

public class Nuget : INuget {
  private const string API_ = "https://api.nuget.org/v3/index.json";
  private const string NUGET_ = "https://api.nuget.org/v3-flatcontainer/";
  private readonly SourceCacheContext cache_;
  private readonly PackageMetadataResource meta_res_;
  private readonly SourceRepository repository_;
  private FindPackageByIdResource resource_;
  private readonly NuGet.Common.ILogger logger_;
  private readonly CancellationToken ct_ =CancellationToken.None;

  public Nuget() {
    repository_ = Repository.Factory.GetCoreV3(API_);
    meta_res_ = repository_.GetResource<PackageMetadataResource>();
    resource_ = repository_.GetResource<FindPackageByIdResource>();
    cache_ = new SourceCacheContext();
    logger_ = NullLogger.Instance;
  }
  public async Task<Artifact> ProcessArtifact(string name) {
    
    Artifact artifact = new Artifact() {
      name = name,
      module = "nuget"
    };
    await ProcessArtifactVersions(artifact);
    return artifact;
  }


  private async Task ProcessArtifactVersions(Artifact artifact) {
    IEnumerable<IPackageSearchMetadata> versions =
        await GetMetadata(artifact.name);
    foreach (IPackageSearchMetadata version in versions) {
      string v = version.Identity.Version.ToString();
      string u = NUGET_ + $"{artifact.name}/{v}/{artifact.name}.{v}.nupkg".ToLower();
      ArtifactVersion a_v = new() {
        artifact_id = artifact.id,
        location = u,
        version = v
      };
      AddDependencies(artifact, a_v, version.DependencySets);
    }
  }
  
  private void AddDependencies(Artifact artifact, ArtifactVersion version, IEnumerable<PackageDependencyGroup> dependencies) {
    if (dependencies == null) return;
    foreach (PackageDependencyGroup x in dependencies) {
      foreach (PackageDependency pkg in x.Packages) {
        artifact.AddDependency(pkg.Id);
        version.AddDependency(pkg.Id);
      }
    }
  }
  private async Task<IEnumerable<IPackageSearchMetadata>> GetMetadata(string id) {
      return await meta_res_.GetMetadataAsync(
        id,
        true,
        false,
        cache_,
        logger_,
        ct_
      );
    }
}