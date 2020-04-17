namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Tenant
{
    public class TenantResult
    {
        public string Slug { get; }

        public string Name { get; set; }

        public string? Logo { get; set; }

        public string[] Tags { get; set; }

        public TenantResult(Data.Entity.Models.Tenant tenant)
        {
            Slug = tenant.Slug;
            Name = tenant.Name;
            Logo = tenant.Logo?.Url;
            Tags = tenant.Tags.Split(',');
        }
    }
}