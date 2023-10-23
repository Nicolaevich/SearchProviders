using Newtonsoft.Json;

namespace SearchProviders.Infrastructure.Http;

public static class Extensions
{
    public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
    {
        string json = await content.ReadAsStringAsync();
        T value = JsonConvert.DeserializeObject<T>(json);
        return value;
    }
}
