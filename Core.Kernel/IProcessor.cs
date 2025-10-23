using Core.Kernel.Messages;
using MassTransit;

namespace Core.Kernel;

public interface IProcessor : IConsumer<ArtifactProcessRequest> {
}