using Core.Services;
using StackExchange.Redis;

namespace Core.Infrastructure;

public class CoreCache : ICoreCache {
  private readonly IConnectionMultiplexer redis_;

  public CoreCache(IConnectionMultiplexer redis) {
    redis_ = redis;
  }

  public async Task<Guid> InitKey(string artifact) {
    Guid context = Guid.NewGuid();
    await AddToCache(artifact, context);
    IDatabase db = redis_.GetDatabase();
    db.KeyExpire(context.ToString(), TimeSpan.FromDays(2));
    return context;
  }

  public async Task<bool> InCache(string artifact, Guid context) {
    IDatabase db = redis_.GetDatabase();
    if (!await db.KeyExistsAsync(context.ToString())) {
      return false;
    }

    return await db.SetContainsAsync(context.ToString(), artifact);
  }

  public async Task<bool> AddToCache(string artifact, Guid context) {
    IDatabase db = redis_.GetDatabase();
    return await db.SetAddAsync(context.ToString(), artifact);
  }
}