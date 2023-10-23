using SearchProviders.API.BL.Services;
using SearchProviders.API.Middlewares;
using SearchProviders.Infrastructure.Cache;
using SearchProviders.Infrastructure.Http;
using SearchProviders.Infrastructure.Http.ProviderOne;
using SearchProviders.Infrastructure.Http.ProviderTwo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ApiHostsSettings>(builder.Configuration.GetSection("ApiHostsSettings"));

builder.Services.AddScoped<ExceptionMiddleware>();

builder.Services.AddSingleton<ICache, InMemoryCache>();

builder.Services.AddScoped<IProviderOneClient, ProviderOneClient>();
builder.Services.AddScoped<IProviderTwoClient, ProviderTwoClient>();

builder.Services.AddScoped<ProviderOneSearchService>();
builder.Services.AddScoped<ProviderTwoSearchService>();

builder.Services.AddScoped<ProviderOneSearchService>();
builder.Services.AddScoped<ProviderTwoSearchService>();

builder.Services.AddScoped(provider =>
{
    return new List<ISearchService>
    {
        provider.GetRequiredService<ProviderOneSearchService>(),
        provider.GetRequiredService<ProviderTwoSearchService>()
    };
});

builder.Services.AddScoped<ISearchService, AggregatedSearchService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
