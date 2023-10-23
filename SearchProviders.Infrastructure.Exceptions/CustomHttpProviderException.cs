namespace SearchProviders.Infrastructure.Exceptions;

public class CustomHttpProviderException : Exception
{
    public CustomHttpProviderException(int code, string message)
        : base("Search provider exception: " + message)
    {
        Code = code;
    }
    public int Code { get; set; }

}
