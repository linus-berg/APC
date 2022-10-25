using APC.Kernel.Messages;
using APC.Services;
using APC.Services.Models;
using MassTransit;
using StackExchange.Redis;

namespace APC.Ingestion; 

public class ProcessingFaultConsumer : IConsumer<Fault<ArtifactProcessRequest>> {
  private readonly IApcDatabase db_;
  public ProcessingFaultConsumer(IApcDatabase db) {
    db_ = db;
  }
  public async Task Consume(ConsumeContext<Fault<ArtifactProcessRequest>> context) {
    Fault<ArtifactProcessRequest> fault = context.Message;
    ArtifactProcessRequest request = fault.Message;
    /*await db_.AddProcessingFault(new ArtifactProcessingFault() {
      name = request.Name,
      module = request.Module,
      time = fault.Timestamp
    });*/
  }
}