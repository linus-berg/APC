using APC.Infrastructure.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace APC.Infrastructure;

public class Database {
  private readonly NpgsqlConnection db_;
  private readonly string DB_STR_ = "Host=localhost;Username=postgres;Password=mysecret;Database=apc-ingestion";

  public Database() {
    db_ = new NpgsqlConnection(DB_STR_);
    Open();
  }

  private void Open() {
    db_.Open();
  }

  ~Database() {
    db_.Close();
  }


  public async Task AddArtifact(Artifact artifact) {
    Artifact db_artifact = await GetArtifactByName(artifact.name);
    if (db_artifact == null) artifact.id = await db_.InsertAsync(artifact);
    foreach (ArtifactVersion version in artifact.versions.Values) await AddArtifactVersion(artifact, version);
    foreach (string dependency in artifact.dependencies) await AddArtifactDependency(artifact, dependency);
  }

  public async Task<ArtifactDependency> AddArtifactDependency(Artifact artifact, string dependency) {
    ArtifactDependency dep = new ArtifactDependency() {
      name = dependency,
      artifact_id = artifact.id
    };
    dep.id = await db_.InsertAsync(dep);
    return dep;
  }
  public async Task AddArtifactVersion(Artifact artifact, ArtifactVersion version) {
    version.artifact_id = artifact.id;
    version.id = await db_.InsertAsync(version);
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
      }
    );
  }

  public async Task<HashSet<string>> GetDependencies(int artifact_id) {
    return (await db_.QueryAsync<string>(@"SELECT name FROM artifact_dependencies WHERE artifact_id = @artifact_id", new {
      artifact_id
    })).ToHashSet();
  }
  public async Task<HashSet<ArtifactVersion>> GetVersions(int artifact_id) {
    HashSet<ArtifactVersion> v = (await db_.QueryAsync<ArtifactVersion>(
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
      }
    )).ToHashSet();
    return v;
  }

  public async Task<ArtifactVersion> GetVersion(int artifact_id, string version) {
    ArtifactVersion v = await db_.QueryFirstOrDefaultAsync<ArtifactVersion>(
      @"
            SELECT 
                * 
            FROM 
                artifact_versions 
            WHERE 
                artifact_id = @artifact_id 
              AND 
                version = @version",
      new {
        artifact_id,
        version
      }
    );
    return v;
  }
}