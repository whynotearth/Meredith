using System;
using System.Collections.Generic;
using Schema.NET;

namespace WhyNotEarth.Meredith.Tenant
{
    public class SeoSchemaService
    {
        private const string ContactEmail = "chris@whynot.earth";
        private const string ContactType = "customer support";
        private const string Country = "Cambodia";

        public string CreateTenantShopSchema(Public.Tenant tenant)
        {
            var shopSchema = new Corporation
            {
                Url = BuildUri(tenant.Company.Slug, "shop/" + tenant.Slug),
                Logo = GetValidUri(tenant.Logo?.Url),
                Name = tenant.Name + " | " + tenant.Company.Name,
                Email = tenant.Owner.Email,
                Image = GetValidUri(tenant.Logo?.Url),
                SameAs = new List<Uri?>
                {
                    GetValidUri(tenant.FacebookUrl),
                    GetValidUri(tenant.Owner?.InstagramUrl)
                },
                Address = new PostalAddress
                {
                    AddressLocality = tenant.Address?.City,
                    AddressRegion = tenant.Address?.State,
                    StreetAddress = tenant.Address?.Street,
                    AddressCountry = Country,
                    PostalCode = tenant.Address?.ZipCode,
                    Url = GetValidUri(tenant.Owner?.GoogleLocation) //Google maps url
                },
                Telephone = tenant.PhoneNumber,
                Description = tenant.Description,
                ContactPoint = new ContactPoint
                {
                    Url = BuildUri(tenant.Company.Slug, "contact"),
                    Email = ContactEmail,
                    ContactType = ContactType
                },
                AlternateName = tenant.Company.Name.ToLower()
            };

            return shopSchema.ToString();
        }

        private static Uri BuildUri(string? companySlug, string? websitePath, string? domainExtension = ".com")
        {
            var baseUri = new UriBuilder
            {
                Scheme = "https",
                Host = companySlug?.ToLower() + domainExtension,
                Path = websitePath?.ToLower()
            };

            return baseUri.Uri;
        }

        private static Uri? GetValidUri(string? path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
                {
                    return new Uri(path);
                }
            }

            return null;
        }
    }
}