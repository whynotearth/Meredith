using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Tenant
{
    public class TenantService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly SlugService _slugService;

        public TenantService(MeredithDbContext dbContext, SlugService slugService)
        {
            _dbContext = dbContext;
            _slugService = slugService;
        }

        public async Task CreateAsync(TenantModel model, User user)
        {
            var slug = _slugService.GetSlug(model.Name);
            var company = await ValidateAsync(model, slug);

            var tenant = GetTenant(model, company, user, slug);

            _dbContext.Tenants.Add(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public Task<List<Data.Entity.Models.Modules.Shop.Tenant>> ListAsync(string companySlug)
        {
            return _dbContext.Tenants
                .Include(item => item.Company)
                .Include(item => item.Logo)
                .Where(s => s.Company.Slug == companySlug.ToLower())
                .ToListAsync();
        }

        private async Task<Company> ValidateAsync(TenantModel model, string slug)
        {
            var company =
                await _dbContext.Companies.FirstOrDefaultAsync(item => item.Slug == model.CompanySlug.ToLower());

            if (company is null)
            {
                throw new RecordNotFoundException($"Company {model.CompanySlug} not found");
            }

            var isSlugDuplicate =
                await _dbContext.Tenants.AnyAsync(item => item.CompanyId == company.Id && item.Slug == slug.ToLower());
            if (isSlugDuplicate)
            {
                throw new InvalidActionException($"The name {slug} is already in use");
            }

            return company;
        }

        private Data.Entity.Models.Modules.Shop.Tenant GetTenant(TenantModel model, Company company, User user, string slug)
        {
            return new Data.Entity.Models.Modules.Shop.Tenant
            {
                CompanyId = company.Id,
                Slug = slug,
                UserId = user.Id,
                Name = model.Name,
                BusinessHours = GetBusinessHours(model.BusinessHours),
                PaymentMethodType = model.PaymentMethodType!.Value,
                NotificationType = model.NotificationType!.Value,
                Description = model.Description
            };
        }

        private List<BusinessHour> GetBusinessHours(List<BusinessHourModel> models)
        {
            return models.Select(item => new BusinessHour
            {
                DayOfWeek = item.DayOfWeek!.Value,
                IsClosed = item.IsClosed!.Value,
                OpeningTime = item.OpeningTime,
                ClosingTime = item.ClosingTime
            }).ToList();
        }
    }
}