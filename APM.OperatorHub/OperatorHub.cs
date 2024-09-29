using System.Text.RegularExpressions;
using APC.Kernel.Exceptions;
using APC.Kernel.Models;
using APM.OperatorHub.Models;
using RestSharp;

namespace APM.OperatorHub;

public class OperatorHub : IOperatorHub {
  private const string C_OP_HUB_ = "https://operatorhub.io/api/operator";
  private readonly RestClient client_ = new(C_OP_HUB_);
  private readonly ILogger<OperatorHub> logger_;

  public OperatorHub(ILogger<OperatorHub> logger) {
    logger_ = logger;
  }

  public async Task<Artifact> ProcessArtifact(Artifact artifact) {
    OperatorMetadata metadata = await GetOperatorMetadata(artifact.id);

    AddVersion(artifact, metadata);

    foreach (OperatorChannel channel in metadata.op.channels) {
      foreach (OperatorVersion version in channel.versions) {
        if (artifact.HasVersion($"{channel.name}-{version.version}")) {
          continue;
        }

        OperatorMetadata mv =
          await GetOperatorVersion(artifact.id, channel.name, version.name);
        AddVersion(artifact, mv);
      }
    }

    return artifact;
  }

  private void AddVersion(Artifact artifact, OperatorMetadata metadata) {
    Operator op = metadata.op;
    string version_str = $"{op.channel}-{op.version}";
    ArtifactVersion version = new() {
      version = version_str
    };
    version.AddFile("containerImage",
                    $"{FixNaming(op.containerImage)}");
    version.AddFile("operator",
                    $"https://operatorhub.io/install/{op.packageName}.yaml");
    artifact.AddVersion(version);
  }

  private static string FixNaming(string name) {
    return !HasHostname(name)
             ? $"docker://docker.io/{name}"
             : $"docker://{name}";
  }

  private static bool HasHostname(string name) {
    bool is_match = Regex.IsMatch(name, @"\w+\.\w+\/");
    return is_match;
  }

  private async Task<OperatorMetadata> GetOperatorMetadata(string id) {
    RestRequest request = new();
    request.AddQueryParameter("packageName", id);
    return await ExecuteRequest<OperatorMetadata>(request);
    //return await client_.GetJsonAsync<OperatorMetadata>($"?packageName={id}");
  }


  private async Task<OperatorMetadata> GetOperatorVersion(
    string id, string channel, string name) {
    RestRequest request = new();
    request.AddQueryParameter("name", name);
    request.AddQueryParameter("channel", channel);
    request.AddQueryParameter("packageName", id);
    return await ExecuteRequest<OperatorMetadata>(request);
  }

  private async Task<T> ExecuteRequest<T>(RestRequest request) {
    try {
      return await client_.GetAsync<T>(request) ??
             throw new InvalidOperationException();
    } catch (TimeoutException ex) {
      logger_.LogError("Timeout error: {Exception}", ex.ToString());
      throw new ArtifactTimeoutException("Timed out!");
    } catch (Exception ex) {
      logger_.LogError("Metadata error: {Exception}", ex.ToString());
      throw new ArtifactMetadataException("Metadata error!");
    }
  }
}