using Web.Api.Helpers;
namespace Web.Api.Extensions;
public static class ResponseExtensions
{
    public static void AddApplicationError(this HttpResponse response, string message)
    {
        response.Headers.Append("Application-Error", Strings.RemoveAllNonPrintableCharacters(message));
        // CORS
        response.Headers.Append("access-control-expose-headers", "Application-Error");
    }
}