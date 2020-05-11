using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Lookups.V1;

namespace WhyNotEarth.Meredith.Sms
{
    public class TwilioService
    {
        private readonly TwilioOptions _options;
        private const string WhatsappOrderSmsTemplateName = "OrderTemplateWhatsapp.txt";

        public TwilioService(IOptions<TwilioOptions> options)
        {
            _options = options.Value;            
        }

        public string GetWhatsappSmsTemplate()
        {
            var assembly = typeof(TwilioService).GetTypeInfo().Assembly;

            var name = assembly.GetManifestResourceNames().FirstOrDefault(item => item.EndsWith(WhatsappOrderSmsTemplateName));
            var stream = assembly.GetManifestResourceStream(name);

            if (stream is null)
            {
                throw new Exception($"Missing resource.");
            }

            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public async Task SendWhatsapp(string body, List<string> recipients)
        {
            TwilioClient.Init(_options.AccountSID, _options.AuthToken);
            foreach (var item in recipients)
            {
                var phoneResult = await PhoneNumberResource.FetchAsync(
                        countryCode: "KH",
                        pathPhoneNumber: new Twilio.Types.PhoneNumber(item)
                    );

                var result = await MessageResource.CreateAsync(
                    body: body,
                    from: new Twilio.Types.PhoneNumber("whatsapp:" + _options.PhoneNumber),
                    to: new Twilio.Types.PhoneNumber("whatsapp:" + phoneResult.PhoneNumber)
                );
            }
        }
    }
}
