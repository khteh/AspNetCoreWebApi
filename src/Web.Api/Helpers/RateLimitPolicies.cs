namespace Web.Api.Helpers;

public static class RateLimitPolicies
{
    /// <summary>
    /// Per-client-IP limit for anonymous credential endpoints (login, refresh token exchange) to slow down brute force and token guessing.
    /// </summary>
    public const string Auth = "auth";
}
