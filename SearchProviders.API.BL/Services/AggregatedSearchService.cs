using SearchProviders.API.BL.Requests;
using SearchProviders.API.BL.Responses;

namespace SearchProviders.API.BL.Services;

public class AggregatedSearchService : ISearchService
{
    private readonly List<ISearchService> _searchServices;

    public AggregatedSearchService(List<ISearchService> searchServices)
    {
        _searchServices = searchServices;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        var tasks = _searchServices.Select(s => s.IsAvailableAsync(cancellationToken));
        var results = await Task.WhenAll(tasks);
        return results.All(result => result);
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        var searchTasks = _searchServices.Select(service => service.SearchAsync(request, cancellationToken));

        var searchResponses = await Task.WhenAll(searchTasks);

        if (!searchResponses.Any(response => response.Routes != null))
            return new SearchResponse();

        var combinedRoutes = searchResponses.SelectMany(response => response.Routes).ToArray();

        return new SearchResponse
        {
            Routes = combinedRoutes,
            MinPrice = combinedRoutes.Min(route => route.Price),
            MaxPrice = combinedRoutes.Max(route => route.Price),
            MinMinutesRoute = (int)combinedRoutes.Min(route => (route.DestinationDateTime - route.OriginDateTime).TotalMinutes),
            MaxMinutesRoute = (int)combinedRoutes.Max(route => (route.DestinationDateTime - route.OriginDateTime).TotalMinutes)
        };
    }
}
