using APC.Kernel.Messages;
using MassTransit;

namespace APC.Kernel;

public interface IProcessor : IConsumer<ArtifactProcessRequest> {
}