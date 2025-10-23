namespace Core.Services;

public interface ICoreCache {
  public Task<Guid> InitKey(string artifact);
  public Task<bool> InCache(string artifact, Guid context);
  public Task<bool> AddToCache(string artifact, Guid context);
}