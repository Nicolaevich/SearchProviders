namespace SearchProviders.API.BL.Responses;

public class SearchResponse
{
    // Mandatory
    // Array of routes
    public Route[] Routes { get; set; }

    // Mandatory
    // The cheapest route
    public decimal MinPrice { get; set; }

    // Mandatory
    // Most expensive route
    public decimal MaxPrice { get; set; }

    // Mandatory
    // The fastest route
    public int MinMinutesRoute { get; set; }

    // Mandatory
    // The longest route
    public int MaxMinutesRoute { get; set; }
}

public class Route
{
    // Mandatory
    // Identifier of the whole route
    public Guid Id { get; set; }

    // Mandatory
    // Start point of route
    public string Origin { get; set; }

    // Mandatory
    // End point of route
    public string Destination { get; set; }

    // Mandatory
    // Start date of route
    public DateTime OriginDateTime { get; set; }

    // Mandatory
    // End date of route
    public DateTime DestinationDateTime { get; set; }

    // Mandatory
    // Price of route
    public decimal Price { get; set; }

    // Mandatory
    // Timelimit. After it expires, route became not actual
    public DateTime TimeLimit { get; set; }
}
