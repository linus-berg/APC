namespace APC.Services;

public interface IApcCache {
  public Task<Guid> InitKey(string artifact);
  public Task<bool> InCache(string artifact, Guid context);
  public Task<bool> AddToCache(string artifact, Guid context);
}