namespace WhyNotEarth.Meredith.Company
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Pages;
    using Pages.Data;

    public class CompanyService
    {
        protected PageDatabase PageDatabase { get; }

        public CompanyService(
            PageDatabase pageDatabase)
        {
            PageDatabase = pageDatabase;
        }

        public async Task<Page> 
            GetCompanyPage(string companyKey, string pageKey)
        {
            await PageDatabase.CacheBust();
            var business = PageDatabase.Businesses
                .FirstOrDefault(b => string.Equals(b.Key, companyKey, StringComparison.CurrentCultureIgnoreCase));
            var page = business?.Pages
                .FirstOrDefault(p => string.Equals(p.Key, pageKey, StringComparison.CurrentCultureIgnoreCase));
            return page;
        }
    }
}