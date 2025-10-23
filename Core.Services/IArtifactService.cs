using Core.Kernel.Messages;
using Core.Kernel.Models;

namespace Core.Services;

public interface IArtifactService {
  public Task<Artifact> AddArtifact(string id, string processor, string filter,
                                    Dictionary<string, string> config,
                                    bool root = false);

  public Task Collect(string location, string processor);
  public Task Collect(ArtifactCollectRequest request);
  public Task Route(Artifact artifact);
  public Task Ingest(Artifact artifact);
  public Task Process(Artifact artifact, Guid? existing_ctx = null);
  public Task<bool> Track(string id, string processor);
  public Task Track();
  public Task Track(string processor_str);
  public Task Validate();
  public Task Validate(Processor processor);
  public Task Validate(string id, string processor_id);
  public Task Validate(Artifact artifact, Processor processor);
}