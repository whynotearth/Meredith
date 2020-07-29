using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace WhyNotEarth.Meredith
{
    public static class UrlHelper
    {
        [return: NotNullIfNotNull("url")]
        public static string? AddQueryString(string? url, Dictionary<string, string> values)
        {
            if (url is null)
            {
                return url;
            }

            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var keyValuePair in values)
            {
                query[keyValuePair.Key] = keyValuePair.Value;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
    }
}