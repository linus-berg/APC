using APC.Kernel.Messages;
using MassTransit;

namespace APC.Kernel; 

public interface ICollector : IConsumer<ArtifactCollectRequest> {
}