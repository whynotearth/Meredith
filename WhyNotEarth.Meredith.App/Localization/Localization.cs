using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;

namespace WhyNotEarth.Meredith.App.Localization
{
    internal class Localization
    {
        public ImmutableList<string> SupportedCultures { get; } = ImmutableList.Create("en-US", "km-KH");
    }

    internal static class LocalizationExtensions
    {
        public static IApplicationBuilder UseCustomLocalization(this IApplicationBuilder app)
        {
            var localization = new Localization();
            var supportedCultures = localization.SupportedCultures.Select(item => new CultureInfo(item)).ToList();

            return app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
        }
    }
}
