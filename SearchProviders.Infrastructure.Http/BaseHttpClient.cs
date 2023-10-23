using SearchProviders.Infrastructure.Exceptions;
using SearchProviders.Infrastructure.Http.Enum;
using System.Net.Http.Json;

namespace SearchProviders.Infrastructure.Http;

public abstract class BaseHttpClient
{
    Service Service { get; }
    protected string HostUrl { get; }

    protected BaseHttpClient(string hostUrl, Service service)
    {
        HostUrl = hostUrl;
        Service = service;
    }

    protected virtual async Task<TResponse> GetRequestAsync<TResponse>(string url, CancellationToken cancellationToken = default)
    {
        var response = await GetHttpRequestAsync(url, cancellationToken);
        return await GetResponseAsync<TResponse>(response);
    }

    protected virtual async Task<TResponse> PostRequestAsync<TResponse, TRequest>(string url, TRequest requestData, CancellationToken cancellationToken = default)
    {
        var response = await PostHttpRequestAsync(url, requestData, cancellationToken);
        return await GetResponseAsync<TResponse>(response);
    }

    private async Task<HttpResponseMessage> GetHttpRequestAsync(string url, CancellationToken cancellationToken = default)
    {
        using (var client = new HttpClient())
        {
            SetHttpClientParams(client);

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            return await client.SendAsync(request, cancellationToken);
        }
    }

    private async Task<HttpResponseMessage> PostHttpRequestAsync<TRequestData>(string url, TRequestData requestData, CancellationToken cancellationToken = default)
    {
        using (var client = new HttpClient())
        {
            SetHttpClientParams(client);

            return await client.PostAsJsonAsync(url, requestData, cancellationToken);
        }
    }

    private void SetHttpClientParams(HttpClient client)
    {
        client.BaseAddress = new Uri(HostUrl);
    }

    private async Task<TResult> GetResponseAsync<TResult>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsJsonAsync<TResult>();

        await ProccesErrorResponseAsync(response);

        throw new CustomHttpProviderException((int)response.StatusCode, "Bad request");
    }

    private async Task ProccesErrorResponseAsync(HttpResponseMessage response)
    {
        var httpResponseContent = await response.Content.ReadAsStringAsync();
        if (!string.IsNullOrEmpty(httpResponseContent))
        {
            throw new CustomHttpProviderException((int)response.StatusCode, httpResponseContent);
        }
    }
}
