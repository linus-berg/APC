using System.Text.RegularExpressions;
using APC.Kernel.Extensions;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace ACM.Router;

public class Router : IConsumer<ArtifactRouteRequest> {
  public async Task Consume(ConsumeContext<ArtifactRouteRequest> context) {
    Artifact artifact = context.Message.Artifact;
    Regex regex = null;
    if (!string.IsNullOrEmpty(artifact.filter)) {
      regex = new Regex(artifact.filter);
    }

    foreach (KeyValuePair<string, ArtifactVersion> kv in artifact.versions) {
      bool collect = regex == null || regex.IsMatch(kv.Key);
      if (!collect) {
        continue;
      }

      foreach (KeyValuePair<string, ArtifactFile> file in kv.Value.files) {
        await context.Collect(file.Value.uri, file.Value.processor);
      }
    }
  }
}