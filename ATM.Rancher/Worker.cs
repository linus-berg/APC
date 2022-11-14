using System.Text.Json;
using APC.Kernel;
using APC.Kernel.Messages;
using RestSharp;

namespace ATM.Rancher;

public class Worker : BackgroundService {
  private readonly RestClient client_ = new($"{Configuration.GetApcVar(ApcVariable.APC_API_HOST)}");
  private readonly ILogger<Worker> logger_;
  private readonly Dictionary<string, RancherProcessor> processors_ = new Dictionary<string, RancherProcessor>();

  public Worker(ILogger<Worker> logger) {
    logger_ = logger;
    processors_["rancher/rke2"] = new RancherProcessor("rancher/rke2", "rke2-images-all.linux-amd64.txt");
    processors_["rancher/rancher"] = new RancherProcessor("rancher/rancher", "rancher-images.txt");
  }

  protected override async Task ExecuteAsync(CancellationToken stopping_token) {
    while (!stopping_token.IsCancellationRequested) {
      try {
        await CheckForReleases();
      }
      catch (Exception e) {
        Console.WriteLine(e);
      }

      await Task.Delay(60 * 1000 * 60, stopping_token);
    }
  }

  private async Task CheckForReleases() {
    foreach (KeyValuePair<string, RancherProcessor> processor in processors_) {
      List<string> new_releases = await processor.Value.CheckReleases();
      foreach (string release_file in new_releases) {
        await CollectRelease(processor.Key, release_file);
      }
    }
  }

  private async Task CollectRelease(string repo, string release) {
    List<string> images = File.ReadLines(release).ToList();
    foreach (string image in images) {
      Console.WriteLine($"Collecting {image}");
      await CollectImage(repo, image.Contains("docker.io") ? image : $"docker.io/{image}");
    }
  }

  private async Task CollectImage(string repo, string image) {
    RestRequest request = new RestRequest("api/artifact/collect", Method.Post);
    request.RequestFormat = DataFormat.Json;
    ArtifactCollectRequest body = new ArtifactCollectRequest() {
      location = $"docker://{image}",
      module = repo
    };
    request.AddBody(JsonSerializer.Serialize(body));
    await client_.ExecuteAsync(request);
  }
}