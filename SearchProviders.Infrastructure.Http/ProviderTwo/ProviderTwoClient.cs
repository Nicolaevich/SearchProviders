using Microsoft.Extensions.Options;
using SearchProviders.Infrastructure.Http.Enum;
using SearchProviders.Infrastructure.Http.ProviderTwo.Requests;
using SearchProviders.Infrastructure.Http.ProviderTwo.Responses;

namespace SearchProviders.Infrastructure.Http.ProviderTwo;

public class ProviderTwoClient : BaseHttpClient, IProviderTwoClient
{
    public ProviderTwoClient(IOptions<ApiHostsSettings> settings)
        : base(settings.Value.ProviderTwo, Service.ProviderTwo)
    { }

    public async Task<bool> PingAsync(CancellationToken cancellationToken)
    {
        return true;
        return await GetRequestAsync<bool>("v1/ping", cancellationToken);
    }

    public async Task<ProviderTwoSearchResponse> SearchAsync(ProviderTwoSearchRequest request, CancellationToken cancellationToken)
    {
        return await PostRequestAsync<ProviderTwoSearchResponse, ProviderTwoSearchRequest>("v1/search", request, cancellationToken);
    }
}
