using Microsoft.AspNetCore.HttpLogging;
namespace Web.Api.Helpers;

/// <summary>
/// HTTP logging fields for endpoints whose request or response bodies carry credentials
/// (passwords, access / refresh tokens, reset codes). Everything in HttpLoggingFields.All except the bodies.
/// Usage: [HttpLogging(SensitiveHttpLogging.Fields)] on the controller or action.
/// </summary>
public static class SensitiveHttpLogging
{
    public const HttpLoggingFields Fields = HttpLoggingFields.All & ~(HttpLoggingFields.RequestBody | HttpLoggingFields.ResponseBody);
}
