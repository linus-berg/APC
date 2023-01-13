using APC.Kernel.Messages;
using APC.Kernel.Models;

namespace APC.Services;

public interface IArtifactService {
  public Task<Artifact> AddArtifact(string id, string processor, string filter,
                                    bool root = false);

  public Task Collect(string location, string processor);
  public Task Collect(ArtifactCollectRequest request);
  public Task Route(Artifact artifact);
  public Task Ingest(Artifact artifact);
  public Task Process(Artifact artifact, Guid? existing_ctx = null);
  public Task<bool> Track(string id, string processor);
  public Task ReTrack();
  public Task Validate();
  public Task Validate(Processor processor);
}