using SearchProviders.API.BL.Requests;
using SearchProviders.API.BL.Responses;
using SearchProviders.Infrastructure.Cache;
using SearchProviders.Infrastructure.Http.ProviderTwo.Requests;
using SearchProviders.Infrastructure.Http.ProviderTwo.Responses;
using SearchProviders.Infrastructure.Http.ProviderTwo;
using SearchProviders.Infrastructure.Exceptions;

namespace SearchProviders.API.BL.Services;

public class ProviderTwoSearchService : ISearchService
{
    private readonly IProviderTwoClient _providerTwoClient;
    private readonly ICache _cache;

    public ProviderTwoSearchService(IProviderTwoClient providerTwoClient, ICache cache)
    {
        _providerTwoClient = providerTwoClient;
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

        var providerTwoRequest = MapToProviderTwoRequest(request);

        var providerTwoResponse = await _providerTwoClient.SearchAsync(providerTwoRequest, cancellationToken);

        var result = MapToSearchResponse(providerTwoResponse);

        _cache.Add(cacheKey, result, TimeSpan.FromMinutes(30));

        return result;
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        return await _providerTwoClient.PingAsync(cancellationToken);
    }

    private ProviderTwoSearchRequest MapToProviderTwoRequest(SearchRequest request)
    {
        return new ProviderTwoSearchRequest
        {
            Departure = request.Origin,
            Arrival = request.Destination,
            DepartureDate = request.OriginDateTime,
            MinTimeLimit = request.Filters?.MinTimeLimit
        };
    }

    private SearchResponse MapToSearchResponse(ProviderTwoSearchResponse providerTwoResponse)
    {
        var routes = providerTwoResponse.Routes.Select(route => new Route
        {
            Id = Guid.NewGuid(),
            Origin = route.Departure.Point,
            Destination = route.Arrival.Point,
            OriginDateTime = route.Departure.Date,
            DestinationDateTime = route.Arrival.Date,
            Price = route.Price,
            TimeLimit = route.TimeLimit
        }).ToArray();

        return new SearchResponse
        {
            Routes = routes,
            MinPrice = routes.Min(route => route.Price),
            MaxPrice = routes.Max(route => route.Price),
            MinMinutesRoute = (int)providerTwoResponse.Routes.Min(route => (route.Arrival.Date - route.Departure.Date).TotalMinutes),
            MaxMinutesRoute = (int)providerTwoResponse.Routes.Max(route => (route.Arrival.Date - route.Departure.Date).TotalMinutes)
        };
    }
}
