namespace APC.Kernel;

public static class Endpoints {
  public static readonly Uri APC_INGEST = new("queue:apc-ingest");
}