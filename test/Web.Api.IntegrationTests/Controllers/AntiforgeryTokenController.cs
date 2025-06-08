// Copyright (c) Martin Costello, 2020. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Net.Mime;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Web.Api.IntegrationTests.Controllers;

/// <summary>
/// A class representing a controller for an HTTP GET resource that returns
/// valid CSRF tokens for use in integration tests. This class cannot be inherited.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class AntiforgeryTokenController : Controller
{
    /// <summary>
    /// The URL for the GET resource.
    /// </summary>
    private const string GetUrl = "_testing/get-xsrf-token";

    /// <summary>
    /// Gets the URI for the resource to get valid antiforgery tokens.
    /// </summary>
    public static Uri GetTokensUri { get; } = new Uri(GetUrl, UriKind.Relative);

    /// <summary>
    /// Returns a JSON object containing valid antiforgery tokens.
    /// </summary>
    /// <param name="antiforgery">The <see cref="IAntiforgery"/> to use.</param>
    /// <param name="options">The <see cref="AntiforgeryOptions"/> to use.</param>
    /// <returns>
    /// An <see cref="AntiforgeryTokens"/> containing valid tokens for antiforgery.
    /// </returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json, Type = typeof(AntiforgeryTokens))]
    [Route(GetUrl, Name = "GetAntiforgeryTokens")]
    public IActionResult GetAntiforgeryTokens(
        [FromServices] IAntiforgery antiforgery,
        [FromServices] IOptions<AntiforgeryOptions> options)
    {
        ArgumentNullException.ThrowIfNull(antiforgery);
        ArgumentNullException.ThrowIfNull(options);

        AntiforgeryTokenSet tokens = antiforgery.GetTokens(HttpContext);

        var model = new AntiforgeryTokens()
        {
            CookieName = options.Value!.Cookie!.Name!,
            CookieValue = tokens.CookieToken!,
            FormFieldName = options.Value.FormFieldName,
            HeaderName = tokens.HeaderName!,
            RequestToken = tokens.RequestToken!,
        };

        return Json(model);
    }
}
