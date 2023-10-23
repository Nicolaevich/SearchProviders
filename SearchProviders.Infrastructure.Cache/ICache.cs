namespace SearchProviders.Infrastructure.Cache;

public interface ICache
{
    void Add<T>(string key, T value, TimeSpan duration);
    T Get<T>(string key);
    void Remove(string key);
}
