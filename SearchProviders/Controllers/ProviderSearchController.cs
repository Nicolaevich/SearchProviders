using Microsoft.AspNetCore.Mvc;
using SearchProviders.API.BL.Requests;
using SearchProviders.API.BL.Services;

namespace SearchProviders.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProviderSearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public ProviderSearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("ping")]
    public async Task<IActionResult> Ping()
        => Ok(await _searchService.IsAvailableAsync(HttpContext.RequestAborted));

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] SearchRequest request)
        => Ok(await _searchService.SearchAsync(request, HttpContext.RequestAborted));
    
}
