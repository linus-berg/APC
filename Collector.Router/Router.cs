using System.Text.RegularExpressions;
using APC.Kernel.Extensions;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;
using Semver;

namespace Collector.Router;

public class Router : IConsumer<ArtifactRouteRequest> {
  private static readonly Predicate<string> S_NO_FILTER_ = s => true;

  public async Task Consume(ConsumeContext<ArtifactRouteRequest> context) {
    Artifact artifact = context.Message.artifact;

    Predicate<string> artifact_filter = CreateFilterFunction(artifact);

    foreach (KeyValuePair<string, ArtifactVersion> kv in artifact.versions) {
      bool collect = artifact_filter(kv.Key);

      if (!collect) {
        continue;
      }

      foreach (KeyValuePair<string, ArtifactFile> file in kv.Value.files) {
        await context.Collect(
          file.Value.uri,
          string.IsNullOrEmpty(file.Value.folder)
            ? artifact.processor
            : file.Value.folder
        );
      }
    }
  }

  private static Predicate<string> CreateFilterFunction(Artifact artifact) {
    if (string.IsNullOrEmpty(artifact.filter)) {
      return S_NO_FILTER_;
    }

    switch (artifact.filter_type) {
      case ArtifactFilterType.SEMVER_RANGE:
        SemVersionRange version_range = SemVersionRange.Parse(artifact.filter);
        return s => {
          SemVersion version = SemVersion.Parse(s, SemVersionStyles.Any);
          return version_range.Contains(version);
        };
      case ArtifactFilterType.REGEX:
        goto default;
      default:
        // fallback to regex, for backwards compatibility
        Regex regex = new(artifact.filter);
        return s => regex.IsMatch(s);
    }
  }
}