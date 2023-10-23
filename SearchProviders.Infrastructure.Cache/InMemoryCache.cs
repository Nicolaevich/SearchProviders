namespace SearchProviders.Infrastructure.Cache;

public class InMemoryCache : ICache
{
    private readonly Dictionary<string, object> cache = new Dictionary<string, object>();

    public void Add<T>(string key, T value, TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(key) || value == null || duration <= TimeSpan.Zero)
            return;

        var cacheEntry = new CacheEntry<T>
        {
            Value = value,
            Expiration = DateTime.UtcNow.Add(duration)
        };

        cache[key] = cacheEntry;
    }

    public T? Get<T>(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || !cache.ContainsKey(key))
            return default(T);

        var cacheEntry = (CacheEntry<T>)cache[key];

        if (cacheEntry.Expiration <= DateTime.UtcNow)
        {
            cache.Remove(key);
            return default(T);
        }

        return cacheEntry.Value;
    }

    public void Remove(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return;

        cache.Remove(key);
    }

    private class CacheEntry<T>
    {
        public T? Value { get; set; }
        public DateTime Expiration { get; set; }
    }
}
