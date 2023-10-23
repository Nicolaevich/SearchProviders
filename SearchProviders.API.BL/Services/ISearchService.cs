using SearchProviders.API.BL.Requests;
using SearchProviders.API.BL.Responses;

namespace SearchProviders.API.BL.Services;

public interface ISearchService
{
    Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken);
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
}
