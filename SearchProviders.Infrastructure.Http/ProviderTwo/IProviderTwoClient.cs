using SearchProviders.Infrastructure.Http.ProviderTwo.Requests;
using SearchProviders.Infrastructure.Http.ProviderTwo.Responses;

namespace SearchProviders.Infrastructure.Http.ProviderTwo;

public interface IProviderTwoClient
{
    Task<ProviderTwoSearchResponse> SearchAsync(ProviderTwoSearchRequest request, CancellationToken cancellationToken);
    Task<bool> PingAsync(CancellationToken cancellationToken);
}
