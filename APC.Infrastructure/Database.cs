using System.Transactions;
using APC.Infrastructure.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace APC.Infrastructure;

public class Database : IDisposable {
  private readonly NpgsqlConnection db_;
  private NpgsqlTransaction transaction_;
  private readonly string DB_STR_ = "Host=localhost;Username=postgres;Password=mysecret;Database=apc-ingestion";

  public Database() {
    db_ = new NpgsqlConnection(DB_STR_);
    Open();
    transaction_ = db_.BeginTransaction();
  }

  private void Open() {
    db_.Open();
  }

  ~Database() {
    Dispose(false);
  }

  public async Task Commit() {
    await transaction_.CommitAsync();
    transaction_ = await db_.BeginTransactionAsync();
  }


  public async Task AddArtifact(Artifact artifact) {
    Artifact db_artifact = await GetArtifactByName(artifact.name);
    if (db_artifact == null) artifact.id = await db_.InsertAsync(artifact, transaction_);
    foreach (ArtifactVersion version in artifact.versions.Values) await AddArtifactVersion(artifact, version);
    foreach (string dependency in artifact.dependencies) await AddArtifactDependency(artifact, dependency);
  }

  public async Task<ArtifactDependency> AddArtifactDependency(Artifact artifact, string dependency) {
    ArtifactDependency dep = new ArtifactDependency() {
      name = dependency,
      artifact_id = artifact.id
    };
    dep.id = await db_.InsertAsync(dep, transaction_);
    return dep;
  }

  public async Task<bool> UpdateArtifact(Artifact current, Artifact updated) {
    return await UpdateArtifactVersions(current, updated);
  }

  public async Task<bool> UpdateArtifactVersions(Artifact current, Artifact updated) {
    Dictionary<string, ArtifactVersion> current_versions = await GetVersions(current.id);
    Dictionary<string, ArtifactVersion> updated_versions = updated.versions;
    bool has_updated = false;  
    foreach (KeyValuePair<string, ArtifactVersion> kv in updated_versions) {
      if (!current_versions.ContainsKey(kv.Key)) {
        await AddArtifactVersion(current, kv.Value);
        has_updated = true;
      }
    }
    return has_updated;
  }
  public async Task AddArtifactVersion(Artifact artifact, ArtifactVersion version) {
    version.artifact_id = artifact.id;
    version.id = await db_.InsertAsync(version, transaction_);
  }

  public async Task<Artifact> GetArtifactByName(string name) {
    return await db_.QueryFirstOrDefaultAsync<Artifact>(@"
        SELECT 
            * 
        FROM 
            artifacts 
        WHERE 
            name = @name",
      new {
        name
      },
      transaction: transaction_
    );
  }

  public async Task<Dictionary<string, ArtifactVersion>> GetVersions(int artifact_id) {
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
        artifact_id,
      },
      transaction: transaction_
    )).ToDictionary(av => av.version);
  }

  public async Task<IEnumerable<Artifact>> GetArtifacts(string module) {
    return await db_.QueryAsync<Artifact>("SELECT * FROM artifacts WHERE module = @module", new { module },
      transaction_);
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

  public void Dispose() {
    Dispose(true);
    GC.SuppressFinalize(this);
  }
}