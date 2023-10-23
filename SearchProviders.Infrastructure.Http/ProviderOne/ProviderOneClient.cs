using Microsoft.Extensions.Options;
using SearchProviders.Infrastructure.Http.Enum;
using SearchProviders.Infrastructure.Http.ProviderOne.Requests;
using SearchProviders.Infrastructure.Http.ProviderOne.Responses;

namespace SearchProviders.Infrastructure.Http.ProviderOne;

public class ProviderOneClient : BaseHttpClient, IProviderOneClient
{
    public ProviderOneClient(IOptions<ApiHostsSettings> settings)
        : base(settings.Value.ProviderOne, Service.ProviderOne)
    { }

    public async Task<bool> PingAsync(CancellationToken cancellationToken)
    {
        return true;
        return await GetRequestAsync<bool>("v1/ping", cancellationToken);
    }

    public async Task<ProviderOneSearchResponse> SearchAsync(ProviderOneSearchRequest request, CancellationToken cancellationToken)
    {
        return await PostRequestAsync<ProviderOneSearchResponse, ProviderOneSearchRequest>("v1/search", request, cancellationToken);
    }
}
