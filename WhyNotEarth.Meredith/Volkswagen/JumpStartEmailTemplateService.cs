using System.IO;
using System.Linq;
using System.Reflection;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartEmailTemplateService
    {
        public string GetEmailTemplate()
        {
            var assembly = typeof(JumpStartService).GetTypeInfo().Assembly;

            var name = assembly.GetManifestResourceNames().FirstOrDefault(item => item.EndsWith("EmailTemplate.html"));
            var stream = assembly.GetManifestResourceStream(name);

            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}