using Slugify;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity;

namespace WhyNotEarth.Meredith.Public
{
    public class SlugService
    {
        private readonly MeredithDbContext _dbContext;

        public SlugService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }        

        public string GetSlug(string name)
        {
            return name.ToLower();
        }

        public async Task<string> UpdateSlug(string name, Data.Entity.Models.Tenant tenant)
        {
            tenant.Slug = new SlugHelper().GenerateSlug(name + " " + tenant.Id);
            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();

            return tenant.Slug;            
        }
    }
}