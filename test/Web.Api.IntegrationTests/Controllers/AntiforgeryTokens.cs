// Copyright (c) Martin Costello, 2020. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Web.Api.IntegrationTests.Controllers;

/// <summary>
/// A class containing valid antiforgery tokens for an ASP.NET Core application.
/// </summary>
public class AntiforgeryTokens
{
    /// <summary>
    /// Gets or sets the name of the cookie to use.
    /// </summary>
    [JsonProperty("cookieName")]
    [JsonPropertyName("cookieName")]
    public string CookieName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value to use for the antiforgery token HTTP cookie.
    /// </summary>
    [JsonProperty("cookieValue")]
    [JsonPropertyName("cookieValue")]
    public string CookieValue { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the form parameter to use for the antiforgery token.
    /// </summary>
    [JsonProperty("formFieldName")]
    [JsonPropertyName("formFieldName")]
    public string FormFieldName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the HTTP request header to use for the antiforgery token.
    /// </summary>
    [JsonProperty("headerName")]
    [JsonPropertyName("headerName")]
    public string HeaderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value to use for the antiforgery token for forms and/or HTTP request headers.
    /// </summary>
    [JsonProperty("requestToken")]
    [JsonPropertyName("requestToken")]
    public string RequestToken { get; set; } = string.Empty;
}
