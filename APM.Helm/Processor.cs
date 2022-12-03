using System.Text.RegularExpressions;
using APC.Kernel;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Helm;

public class Processor : IProcessor {
  private readonly Helm helm_;

  public Processor(Helm helm) {
    helm_ = helm;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await helm_.ProcessArtifact(artifact);
    ArtifactProcessedRequest request = new() {
      Artifact = artifact,
      Context = context.Message.ctx
    };
    foreach (ArtifactDependency dependency in artifact.dependencies) {
      if (dependency.processor == "container") {
        string container = dependency.id;
        request.AddCollectRequest(FixNaming(container), dependency.processor);
        artifact.dependencies.Remove(dependency);
      }
    }

    await context.Send(Endpoints.APC_INGEST_PROCESSED, request);
  }

  private static string FixNaming(string name) {
    return !HasHostname(name)
             ? $"docker://docker.io/{name}"
             : $"docker://{name}";
  }

  private static bool HasHostname(string name) {
    bool is_match = Regex.IsMatch(name, @"\w+\.\w+\/");
    return is_match;
  }
}