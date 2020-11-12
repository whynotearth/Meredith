using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Services;

namespace WhyNotEarth.Meredith.Identity.Notifications
{
    internal class ConfirmEmailNotification : Notification
    {
        private readonly Company _company;
        private readonly IResourceService _resourceService;
        private readonly string _url;
        private readonly User _user;

        public ConfirmEmailNotification(Company company, User user, string url, IResourceService resourceService) :
            base(company)
        {
            _company = company;
            _user = user;
            _url = url;
            _resourceService = resourceService;
        }

        public override string GetMessage(NotificationType notificationType)
        {
            var templateName = _company.Slug == BrowTricksCompany.Slug
                ? "browtricks-email-confirm.html"
                : "general-email-confirm.html";

            var template = _resourceService.Get(templateName, new Dictionary<string, string>
            {
                {"{{company}}", _company.Name.First().ToString().ToUpper() + _company.Name[1..]},
                {"{{email}}", _user.Email},
                {"{{url}}", HtmlEncoder.Default.Encode(_url)}
            });

            return template;
        }

        public override string GetSubject()
        {
            return "Please confirm your email";
        }
    }
}