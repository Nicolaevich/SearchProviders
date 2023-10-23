using SearchProviders.Infrastructure.Http.ProviderOne.Requests;
using SearchProviders.Infrastructure.Http.ProviderOne.Responses;

namespace SearchProviders.Infrastructure.Http.ProviderOne;

public interface IProviderOneClient
{
    Task<ProviderOneSearchResponse> SearchAsync(ProviderOneSearchRequest request, CancellationToken cancellationToken);
    Task<bool> PingAsync(CancellationToken cancellationToken);
}
