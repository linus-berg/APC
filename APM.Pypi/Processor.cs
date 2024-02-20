using APC.Kernel;
using APC.Kernel.Messages;
using APC.Kernel.Models;
using MassTransit;

namespace APM.Pypi;

public class Processor : IProcessor {
  private readonly IPypi pypi_;

  public Processor(IPypi pypi) {
    pypi_ = pypi;
  }

  public async Task Consume(ConsumeContext<ArtifactProcessRequest> context) {
    Artifact artifact = context.Message.artifact;
    await pypi_.ProcessArtifact(artifact);
    await context.Send(Endpoints.S_APC_INGEST_PROCESSED,
                       new ArtifactProcessedRequest {
                         context = context.Message.ctx,
                         artifact = artifact
                       });
  }
}