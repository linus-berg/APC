using APC.Kernel.Models;
using APC.Services;
using MongoDB.Driver;

namespace APC.Infrastructure;

public class MongoDatabase : IApcDatabase {
  private readonly MongoClient client_;
  private readonly IMongoDatabase database_;

  public MongoDatabase() {
    string c_str = Environment.GetEnvironmentVariable("APC_MONGO_STR");
    client_ = new MongoClient(c_str);
    database_ = client_.GetDatabase("apc");
  }

  public async Task AddArtifact(Artifact artifact) {
    IMongoCollection<Artifact> collection =
      GetCollection<Artifact>(artifact.processor);
    await collection.InsertOneAsync(artifact);
  }

  public async Task<bool> UpdateArtifact(Artifact artifact) {
    IMongoCollection<Artifact> collection =
      GetCollection<Artifact>(artifact.processor);
    ReplaceOneResult result =
      await collection.ReplaceOneAsync(a => a.id == artifact.id, artifact);
    return result.IsAcknowledged;
  }

  public async Task<IEnumerable<string>> GetProcessors() {
    return await (await database_.ListCollectionNamesAsync()).ToListAsync();
  }

  public async Task<Artifact> GetArtifact(string id, string processor) {
    IAsyncCursor<Artifact> cursor =
      await GetCollection<Artifact>(processor).FindAsync(a => a.id == id);
    return await cursor.FirstOrDefaultAsync();
  }

  public async Task<IEnumerable<Artifact>> GetArtifacts(
    string processor, bool only_roots = true) {
    IAsyncCursor<Artifact> cursor =
      await GetCollection<Artifact>(processor)
        .FindAsync(a => a.root || !only_roots);
    return await cursor.ToListAsync();
  }

  public async Task<bool> DeleteArtifact(Artifact artifact) {
    IMongoCollection<Artifact> collection =
      GetCollection<Artifact>(artifact.processor);
    Artifact a =
      await collection.FindOneAndDeleteAsync(a => a.id == artifact.id);
    return a != null;
  }

  private IMongoCollection<T> GetCollection<T>(string collection) {
    return database_.GetCollection<T>(collection);
  }

  public async Task AddProcessor(ArtifactProcessor processor) {
    IMongoCollection<ArtifactProcessor> collection =
      GetCollection<ArtifactProcessor>("processors");

    await collection.InsertOneAsync(processor);
  }

  public async Task<bool> ArtifactExists(string id, string processor) {
    return await GetArtifact(id, processor) != null;
  }
}