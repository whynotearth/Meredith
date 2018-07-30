namespace WhyNotEarth.Meredith.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class PageDatabase
    {
        public List<Business> Businesses { get; set; }

        public DateTime LastCacheLoad { get; set; }
        
        protected PageDatabaseOptions PageDatabaseOptions { get; }

        public PageDatabase(IOptions<PageDatabaseOptions> pageDatabaseOptions)
        {
            PageDatabaseOptions = pageDatabaseOptions.Value;
        }

        public async Task Load()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(PageDatabaseOptions.Url);
            response.EnsureSuccessStatusCode();
            var database = JsonConvert.DeserializeObject<DatabaseRoot>(await response.Content.ReadAsStringAsync());
            Businesses = database.Businesses;
        }

        public async Task CacheBust()
        {
            if (Businesses == null || Businesses.Count == 0 || DateTime.UtcNow.Subtract(LastCacheLoad).Minutes > 60)
            {
                await Load();
            }
        }
    }
}