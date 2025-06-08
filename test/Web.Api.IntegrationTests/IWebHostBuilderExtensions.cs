// Copyright (c) Martin Costello, 2020. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.ComponentModel;
using Microsoft.AspNetCore.Hosting;
using Web.Api.IntegrationTests.Controllers;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A class containing extension methods for the <see cref="IWebHostBuilder"/> interface. This class cannot be inherited.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class IWebHostBuilderExtensions
{
    /// <summary>
    /// Configures an HTTP GET resource for obtaining valid antiforgery tokens.
    /// </summary>
    /// <param name="builder">The <see cref="IWebHostBuilder"/> to configure.</param>
    /// <returns>
    /// The <see cref="IWebHostBuilder"/> specified by <paramref name="builder"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="builder"/> is <see langword="null"/>.
    /// </exception>
    public static IWebHostBuilder ConfigureAntiforgeryTokenResource(this IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.ConfigureServices((services) =>
        {
            services.AddControllers()
                    .AddApplicationPart(typeof(AntiforgeryTokenController).Assembly)
                    .AddControllersAsServices();
        });
    }
}
