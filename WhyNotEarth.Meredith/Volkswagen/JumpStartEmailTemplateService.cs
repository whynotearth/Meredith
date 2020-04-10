using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartEmailTemplateService
    {
        private const string EmailTemplateFileName = "EmailTemplate.html";

        public string GetEmailTemplate()
        {
            var assembly = typeof(JumpStartService).GetTypeInfo().Assembly;
            
            var name = assembly.GetManifestResourceNames().FirstOrDefault(item => item.EndsWith(EmailTemplateFileName));
            var stream = assembly.GetManifestResourceStream(name);

            if (stream is null)
            {
                throw new Exception($"Missing {EmailTemplateFileName} resource.");
            }

            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
    }
}