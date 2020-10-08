using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WhyNotEarth.Meredith.Services
{
    internal class ResourceService : IResourceService
    {
        public string Get(string resourceName)
        {
            var assembly = typeof(ResourceService).GetTypeInfo().Assembly;

            var name = assembly.GetManifestResourceNames().SingleOrDefault(item => item.EndsWith(resourceName));

            if (name is null)
            {
                throw new Exception($"Missing {resourceName} resource.");
            }

            var stream = assembly.GetManifestResourceStream(name);

            if (stream is null)
            {
                throw new Exception($"Missing {resourceName} resource.");
            }

            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}