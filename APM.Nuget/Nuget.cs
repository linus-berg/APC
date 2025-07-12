using APC.Kernel.Exceptions;
using APC.Kernel.Models;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using ILogger = NuGet.Common.ILogger;

namespace APM.Nuget;

public class Nuget : INuget {
  private const string C_API_ = "https://api.nuget.org/v3/index.json";
  private const string C_NUGET_ = "https://api.nuget.org/v3-flatcontainer/";
  private readonly SourceCacheContext cache_;
  private readonly CancellationToken ct_ = CancellationToken.None;
  private readonly ILogger logger_;
  private readonly PackageMetadataResource meta_res_;
  private readonly SourceRepository repository_;
  private FindPackageByIdResource resource_;

  public Nuget() {
    repository_ = Repository.Factory.GetCoreV3(C_API_);
    meta_res_ = repository_.GetResource<PackageMetadataResource>();
    resource_ = repository_.GetResource<FindPackageByIdResource>();
    cache_ = new SourceCacheContext();
    logger_ = NullLogger.Instance;
  }

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    await ProcessArtifactVersions(artifact);
    return artifact;
  }


  private async Task ProcessArtifactVersions(Artifact artifact) {
    IEnumerable<IPackageSearchMetadata> versions =
      await GetMetadata(artifact.id);

    if (versions == null) {
      throw new ArtifactTimeoutException(
        $"Metadata fetch failed for {artifact.id}"
      );
    }

    foreach (IPackageSearchMetadata version in versions) {
      string v = version.Identity.Version.ToString();
      string u = C_NUGET_ +
                 $"{artifact.id}/{v}/{artifact.id}.{v}.nupkg".ToLower();
      if (artifact.HasVersion(v)) {
        continue;
      }

      ArtifactVersion a_v = new() {
        version = v
      };
      a_v.AddFile("nupkg", u);
      AddDependencies(artifact, version.DependencySets);
      artifact.AddVersion(a_v);
    }
  }

  private void AddDependencies(Artifact artifact,
                               IEnumerable<PackageDependencyGroup>
                                 dependencies) {
    if (dependencies == null) {
      throw new ArtifactMetadataException("No versions found!");
    }

    foreach (PackageDependencyGroup pdg in dependencies) {
      foreach (PackageDependency pkg in pdg.Packages) {
        artifact.AddDependency(pkg.Id, artifact.processor);
      }
    }
  }

  private async Task<IEnumerable<IPackageSearchMetadata>>
    GetMetadata(string id) {
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