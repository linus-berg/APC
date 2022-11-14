using System.Data;
using APC.Services;
using APC.Services.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;

namespace APC.Infrastructure;

public class ApcDatabase : IApcDatabase {
  private static readonly string DB_STR_;
  private readonly NpgsqlConnection db_;
  private NpgsqlTransaction transaction_;

  static ApcDatabase() {
    string c_str = Environment.GetEnvironmentVariable("APC_PGSQL_STR");
    DB_STR_ = c_str ?? throw new NoNullAllowedException("Database connection string is null, set APC_PGSQL_STR.");
  }

  public ApcDatabase() {
    db_ = new NpgsqlConnection(DB_STR_);
    Open();
    transaction_ = db_.BeginTransaction();
  }

  public void Dispose() {
    Dispose(true);
    GC.SuppressFinalize(this);
  }

  public async Task<bool> DeleteArtifact(Artifact artifact) {
    return await db_.DeleteAsync(artifact);
  }

  public async Task Commit() {
    await transaction_.CommitAsync();
    transaction_ = await db_.BeginTransactionAsync();
  }

  public async Task AddArtifact(Artifact artifact) {
    Artifact db_artifact = await GetArtifactByName(artifact.name, artifact.module);
    if (db_artifact == null) artifact.id = await db_.InsertAsync(artifact, transaction_);
    foreach (ArtifactVersion version in artifact.versions.Values) await AddArtifactVersion(artifact, version);
    foreach (ArtifactDependency dependency in artifact.dependencies) await AddArtifactDependency(artifact, dependency);
  }

  public async Task<int> AddProcessingFault(ArtifactProcessingFault fault) {
    int id = await db_.InsertAsync(fault);
    fault.id = id;
    return id;
  }

  public async Task<bool> UpdateArtifact(Artifact artifact) {
    return await db_.UpdateAsync(artifact);
  }

  public async Task<bool> UpdateArtifact(Artifact current, Artifact updated) {
    return await UpdateArtifactVersions(current, updated);
  }

  public async Task<IEnumerable<string>> GetModules() {
    return await db_.QueryAsync<string>("SELECT DISTINCT module FROM artifacts");
  }

  public async Task<Artifact> GetArtifactByName(string name, string module) {
    return await db_.QueryFirstOrDefaultAsync<Artifact>(@"
        SELECT 
            * 
        FROM 
            artifacts 
        WHERE 
            name = @name AND module = @module",
      new {
        name,
        module
      },
      transaction_
    );
  }

  public async Task<IEnumerable<Artifact>> GetArtifacts(string module) {
    Dictionary<int, Artifact> artifacts = new();
    return (await db_.QueryAsync<Artifact, ArtifactVersion, Artifact>(@"
        SELECT 
            * 
        FROM 
            artifacts 
        INNER JOIN
                artifact_versions ON artifact_id = artifacts.id
        WHERE 
            module = @module
        ", (artifact, version) => {
        if (!artifacts.TryGetValue(artifact.id, out Artifact entry)) {
          entry = artifact;
          entry.versions = new Dictionary<string, ArtifactVersion>();
          artifacts.Add(artifact.id, entry);
        }

        if (version != null) entry.versions.Add(version.version, version);

        return entry;
      },
      new { module },
      transaction_
    )).Distinct();
  }

  public async Task<IEnumerable<Artifact>> GetRoots(string module) {
    return await db_.QueryAsync<Artifact>("SELECT * FROM artifacts WHERE module = @module and root = true",
      new { module },
      transaction_);
  }

  private void Open() {
    db_.Open();
  }

  ~ApcDatabase() {
    Dispose(false);
  }

  private async Task<ArtifactDependency> AddArtifactDependency(Artifact artifact, ArtifactDependency dependency) {
    dependency.artifact_id = artifact.id;
    dependency.id = await db_.InsertAsync(dependency, transaction_);
    return dependency;
  }

  private async Task<bool> UpdateArtifactVersions(Artifact current, Artifact updated) {
    Dictionary<string, ArtifactVersion> current_versions = await GetVersions(current.id);
    Dictionary<string, ArtifactVersion> updated_versions = updated.versions;
    bool has_updated = false;
    foreach (KeyValuePair<string, ArtifactVersion> kv in updated_versions)
      if (!current_versions.ContainsKey(kv.Key)) {
        await AddArtifactVersion(current, kv.Value);
        has_updated = true;
      }

    return has_updated;
  }

  private async Task AddArtifactVersion(Artifact artifact, ArtifactVersion version) {
    version.artifact_id = artifact.id;
    version.id = await db_.InsertAsync(version, transaction_);
  }

  private async Task<Dictionary<string, ArtifactVersion>> GetVersions(int artifact_id) {
    return (await db_.QueryAsync<ArtifactVersion>(
      @"
            SELECT 
                * 
            FROM 
                artifact_versions 
            WHERE 
                artifact_id = @artifact_id
         ",
      new {
        artifact_id
      },
      transaction_
    )).ToDictionary(av => av.version);
  }

  private void ReleaseUnmanagedResources() {
    // TODO release unmanaged resources here
  }

  private void Dispose(bool disposing) {
    ReleaseUnmanagedResources();
    if (disposing) {
      transaction_.Dispose();
      db_.Dispose();
    }
  }
}