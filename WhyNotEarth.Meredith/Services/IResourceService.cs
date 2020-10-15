using System.Collections.Generic;

namespace WhyNotEarth.Meredith.Services
{
    public interface IResourceService
    {
        string Get(string resourceName);

        string Get(string resourceName, Dictionary<string, string> replaceValues);
    }
}