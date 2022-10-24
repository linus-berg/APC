using System.Text.RegularExpressions;
using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services.Models;
using MassTransit;

namespace ACM.Router; 

public class Router : IRouter {

  public Router() {
  }

  public async Task Consume(ConsumeContext<ArtifactRouteRequest> context) {
    Artifact artifact = context.Message.Artifact;
    Regex regex = null;
    if (!string.IsNullOrEmpty(artifact.filter)) {
      regex = new Regex(artifact.filter);
    }
    foreach (KeyValuePair<string, ArtifactVersion> kv in artifact.versions) {
      bool collect = regex == null || regex.IsMatch(kv.Key);
      if (collect) {
        await Collect(context, kv.Value.location, artifact.module);
      }
    }
  }

  private async Task Collect(ConsumeContext<ArtifactRouteRequest> context, string location, string module) {
    await context.Send(new Uri($"queue:{GetCollectorModule(location)}"), new ArtifactCollectRequest() {
      location = location,
      module = module
    });
  }

  private string GetCollectorModule(string location) {
    Uri uri = new Uri(location);
    string scheme = uri.Scheme;
    return $"acm-{scheme}";
  }
}