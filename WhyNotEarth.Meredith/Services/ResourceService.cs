using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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

        public string Get(string resourceName, Dictionary<string, string> replaceValues)
        {
            var template = Get(resourceName);

            template = Replace(template, replaceValues);

            return template;
        }

        private string Replace(string source, Dictionary<string, string> values)
        {
            var stringBuilder = new StringBuilder(source);

            foreach (var keyValue in values)
            {
                stringBuilder.Replace(keyValue.Key, keyValue.Value);
            }

            return stringBuilder.ToString();
        }
    }
}