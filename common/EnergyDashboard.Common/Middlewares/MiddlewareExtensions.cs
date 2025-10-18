using System;
using System.Collections.Generic;
using System.Text;
using EnergyDashboard.Common.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseLicenceMiddleware(this IApplicationBuilder builder) =>
            builder.UseMiddleware<LicenceMiddleware>();
    }
}
