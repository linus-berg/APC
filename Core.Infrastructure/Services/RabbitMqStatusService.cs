using Core.Infrastructure.Models.RabbitMq;
using Core.Kernel;
using Core.Kernel.Models;
using Core.Services;
using RestSharp;
using RestSharp.Authenticators;

namespace Core.Infrastructure.Services;

public class RabbitMqStatusService : IStatusService {
  private RestClient client_;

  public RabbitMqStatusService() {
    RestClientOptions options = new() {
      BaseUrl = new Uri(
        Configuration.GetBackpackVariable(CoreVariables.BP_RABBIT_MQ_API) ??
        throw new InvalidOperationException()
      ),
      Authenticator = new HttpBasicAuthenticator(
        Configuration.GetBackpackVariable(CoreVariables.BP_RABBIT_MQ_USER) ??
        throw new InvalidOperationException(),
        Configuration.GetBackpackVariable(CoreVariables.BP_RABBIT_MQ_PASS) ??
        throw new InvalidOperationException()
      )
    };
    client_ = new RestClient(options);
  }

  public async Task<List<QueueStatus>> QueueStatus() {
    List<RabbitMqQueue>? queues =
      await client_.GetAsync<List<RabbitMqQueue>>("api/queues");
    if (queues == null) {
      return new();
    }
    
    List<QueueStatus> queue_statuses = new();
    foreach (RabbitMqQueue queue in queues) {
      if (queue.name.Contains("error")) {
        continue;
      }
      queue_statuses.Add(new QueueStatus() {
        name = queue.name,
        consumers = queue.consumers,
        messages = queue.messages,
        avg_egress_rate = queue.backing_queue_status?.avg_egress_rate,
        avg_ingress_rate = queue.backing_queue_status?.avg_ingress_rate
      });
    }
    return queue_statuses;
  }
}