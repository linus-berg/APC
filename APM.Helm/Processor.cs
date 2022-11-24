using APC.Kernel;
using APC.Kernel.Messages;
using APC.Services.Models;
using MassTransit;

namespace APM.Helm;

public class Processor : IProcessor {
  private readonly Helm helm_;

  public Processor(Helm helm) {
    helm_ = helm;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    string name = context.Message.Name;
    Artifact artifact = await helm_.ProcessArtifact(name);
    ArtifactProcessedRequest request = new() {
      Context = context.Message.Context,
      Artifact = artifact
    };
    foreach (ArtifactDependency dependency in artifact.dependencies) {
      if (dependency.module == "container") {
        request.AddCollectRequest($"docker://docker.io/{dependency.name}",
                                  dependency.module);
        artifact.dependencies.Remove(dependency);
      }
    }

    await context.Send(Endpoints.APC_INGEST_PROCESSED, request);
  }
}