namespace Core.Kernel;

public static class Endpoints {
  public static readonly Uri S_GATEWAY_INGEST_PROCESSED =
    new("queue:gateway-ingest-processed");

  public static readonly Uri S_GATEWAY_INGEST_PROCESSED_RAW =
    new("queue:gateway-ingest-processed-raw");

  public static readonly Uri S_COLLECTOR_ROUTER = new("queue:collector-router");

  public static readonly Uri S_GATEWAY_INGEST_UNPROCESSED =
    new("queue:gateway-ingest-unprocessed");
}