using ACM.Kernel;
using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;

namespace ACM.Git;

public class Collector : ICollector {
  private readonly string dir_;
  private readonly FileSystem fs_ = new();
  private readonly Git git_;

  public Collector() {
    dir_ = fs_.GetModuleDir("git", true);
    git_ = new Git(dir_);
  }

  public async Task Consume(ConsumeContext<ArtifactCollectRequest> context) {
    string location = context.Message.location;
    string module = context.Message.module;
    await git_.Mirror(location);
  }
}