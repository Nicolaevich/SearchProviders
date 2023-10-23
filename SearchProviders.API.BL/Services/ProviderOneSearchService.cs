using SearchProviders.API.BL.Requests;
using SearchProviders.API.BL.Responses;
using SearchProviders.Infrastructure.Cache;
using SearchProviders.Infrastructure.Exceptions;
using SearchProviders.Infrastructure.Http.ProviderOne;
using SearchProviders.Infrastructure.Http.ProviderOne.Requests;
using SearchProviders.Infrastructure.Http.ProviderOne.Responses;

namespace SearchProviders.API.BL.Services;

public class ProviderOneSearchService : ISearchService
{
    private readonly IProviderOneClient _providerOneClient;
    private readonly ICache _cache;

    public ProviderOneSearchService(IProviderOneClient providerOneClient, ICache cache)
    {
        _providerOneClient = providerOneClient;
        _cache = cache;
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
            throw new CustomSearchServiceException("Request is empty");

        string cacheKey = $"{request.Origin}_{request.Destination}_{request.OriginDateTime}";

        if (request.Filters != null && request.Filters.OnlyCached == true)
        {
            var cachedData = _cache.Get<SearchResponse>(cacheKey);
            if (cachedData != null)
            {
                return cachedData;
            }
            return new SearchResponse();
        }

        var providerOneRequest = MapToProviderOneRequest(request);

        var providerOneResponse = await _providerOneClient.SearchAsync(providerOneRequest, cancellationToken);

        var result = MapToSearchResponse(providerOneResponse);

        _cache.Add(cacheKey, result, TimeSpan.FromMinutes(30)); 

        return result;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        return await _providerOneClient.PingAsync(cancellationToken);
    }

    private ProviderOneSearchRequest MapToProviderOneRequest(SearchRequest request)
    {
        return new ProviderOneSearchRequest
        {
            From = request.Origin,
            To = request.Destination,
            DateFrom = request.OriginDateTime,
            DateTo = request.Filters?.DestinationDateTime,
            MaxPrice = request.Filters?.MaxPrice
        };
    }

    private SearchResponse MapToSearchResponse(ProviderOneSearchResponse providerOneResponse)
    {
        var routes = providerOneResponse.Routes.Select(route => new Route
        {
            Id = Guid.NewGuid(),
            Origin = route.From,
            Destination = route.To,
            OriginDateTime = route.DateFrom,
            DestinationDateTime = route.DateTo,
            Price = route.Price,
            TimeLimit = route.TimeLimit
        }).ToArray();

        return new SearchResponse
        {
            Routes = routes,
            MinPrice = routes.Min(route => route.Price),
            MaxPrice = routes.Max(route => route.Price),
            MinMinutesRoute = (int)routes.Min(route => (route.DestinationDateTime - route.OriginDateTime).TotalMinutes),
            MaxMinutesRoute = (int)routes.Max(route => (route.DestinationDateTime - route.OriginDateTime).TotalMinutes)
        };
    }
}
