namespace APC.Kernel;

public static class Endpoints {
  public static readonly Uri APC_INGEST_PROCESSED = new("queue:apc-ingest-processed");
  public static readonly Uri APC_INGEST_UNPROCESSED = new("queue:apc-ingest-unprocessed");
}