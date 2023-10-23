using Newtonsoft.Json;
using SearchProviders.Infrastructure.Exceptions;
using SearchProviders.Infrastructure.Exceptions.ResponseModels;

namespace SearchProviders.API.Middlewares;

public class ExceptionMiddleware : IMiddleware
{
    public ExceptionMiddleware()
    {
        
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (CustomSearchServiceException ex)
        {
            // Exception can be logged

            SetCommonHeaders(context, 400);

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ExceptionResponseModel
            {
                ExceptionMessage = ex.Message
            }));
        }
        catch (CustomHttpProviderException ex)
        {
            // Exception can be logged

            SetCommonHeaders(context, 503);

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ExceptionResponseModel
            {
                ExceptionMessage = ex.Message
            }));
        }
        catch (Exception ex)
        {
            // Exception can be logged
            SetCommonHeaders(context, 500);

            await context.Response.WriteAsync(JsonConvert.SerializeObject(new ExceptionResponseModel
            {
                ExceptionMessage = ex.Message
            }));
        }
    }

    private void SetCommonHeaders(HttpContext context, int statusCode)
    {
        context.Response.StatusCode = statusCode;
    }
}
