using Core.Kernel.Messages;
using Core.Services;
using MassTransit;

namespace Core.Gateway;

public class
  ProcessingFaultConsumer : IConsumer<Fault<ArtifactProcessRequest>> {
  private readonly ICoreDatabase db_;

  public ProcessingFaultConsumer(ICoreDatabase db) {
    db_ = db;
  }

  public async Task Consume(
    ConsumeContext<Fault<ArtifactProcessRequest>> context) {
    Fault<ArtifactProcessRequest> fault = context.Message;
    ArtifactProcessRequest request = fault.Message;
    /*await db_.AddProcessingFault(new ArtifactProcessingFault() {
      name = request.Name,
      module = request.Module,
      time = fault.Timestamp
    });*/
  }
}