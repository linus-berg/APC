using Core.Kernel.Messages;
using MassTransit;

namespace Core.Kernel;

public interface ICollector : IConsumer<ArtifactCollectRequest> {
}