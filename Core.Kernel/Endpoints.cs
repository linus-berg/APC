namespace Core.Kernel;

public static class Endpoints {
  public static readonly Uri S_APC_INGEST_PROCESSED =
    new("queue:apc-ingest-processed");

  public static readonly Uri S_APC_INGEST_PROCESSED_RAW =
    new("queue:apc-ingest-processed-raw");

  public static readonly Uri S_APC_ACM_ROUTER = new("queue:acm-router");

  public static readonly Uri S_APC_INGEST_UNPROCESSED =
    new("queue:apc-ingest-unprocessed");
}