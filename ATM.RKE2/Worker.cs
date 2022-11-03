using APC.Kernel;
using APC.Kernel.Messages;
using MassTransit;
using RestSharp;

namespace ATM.RKE2;

public class Worker : BackgroundService {
  private readonly ILogger<Worker> _logger;

  private readonly RestClient client_ = new($"{Configuration.GetAPCVar(ApcVariable.APC_API_HOST)}");
  private readonly RancherProcessor processor_ = new RancherProcessor();

  public Worker(ILogger<Worker> logger) {
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    while (!stoppingToken.IsCancellationRequested) {
      await CheckForReleases();
      await Task.Delay(60 * 1000 * 60, stoppingToken);
    }
  }

  private async Task CheckForReleases() {
    List<string> new_releases = await processor_.CheckReleases();
    foreach (string release_file in new_releases) {
      await CollectRelease(release_file);
    }
  }

  private async Task CollectRelease(string release) {
    List<string> images = File.ReadLines(release).ToList();
    
    foreach (string image in images) {
      Console.WriteLine($"Collecting {image}");
      await CollectImage(image);
    }
  }
  
  private async Task CollectImage(string container) {
    RestRequest request = new RestRequest("api/artifact/collect");
    request.RequestFormat = DataFormat.Json;
    request.AddJsonBody(new ArtifactCollectRequest() {
      location = $"docker://{container}",
      module = "RKE2"
    });
    await client_.PostAsync(request);
  }
  
  
  
}
