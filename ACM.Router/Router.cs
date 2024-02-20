using System.Text.RegularExpressions;
using APC.Kernel.Extensions;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;
using Semver;

namespace ACM.Router;

public class Router : IConsumer<ArtifactRouteRequest> {
  public async Task Consume(ConsumeContext<ArtifactRouteRequest> context) {
    Artifact artifact = context.Message.artifact;

    Predicate<string> artifactFilter = CreateFilterFunction(artifact);

    foreach (KeyValuePair<string, ArtifactVersion> kv in artifact.versions) {
      bool collect = artifactFilter(kv.Key);

      if (!collect) {
        continue;
      }

      foreach (KeyValuePair<string, ArtifactFile> file in kv.Value.files) {
        await context.Collect(file.Value.uri,
                              string.IsNullOrEmpty(file.Value.folder)
                                ? artifact.processor
                                : file.Value.folder);
      }
    }
  }

  private static Predicate<string> NO_FILTER = s => true;

  private static Predicate<string> CreateFilterFunction(Artifact artifact) {
    if (string.IsNullOrEmpty(artifact.filter)) {
        return NO_FILTER;
    }

    switch (artifact.filterType) {
      case ArtifactFilterType.SemverRange:
        SemVersionRange versionRange = SemVersionRange.Parse(artifact.filter);
        return (s) => {
          SemVersion version = SemVersion.Parse(s, SemVersionStyles.Any);
          return versionRange.Contains(version);
        };
      case ArtifactFilterType.Regex:
        goto default;
      default:
        // fallback to regex, for backwards compatibility
        Regex regex = new Regex(artifact.filter);
        return (s) => regex.IsMatch(s);
    }
  }
}
