using System;
namespace Web.Api.Infrastructure.Extensions;
public static class ExceptionExtensions
{
    public static string GetInnerMessage(this Exception ex)
            => ex.InnerException?.InnerException?.Message ?? (ex.InnerException?.Message ?? ex.Message);
}