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

        public async Task<string> CreateAsync(string companySlug, TenantModel model, User user)
        {
            var company = await ValidateAsync(companySlug);

            var tenant = GetTenant(model, company, user);

            _dbContext.Tenants.Add(tenant);
            await _dbContext.SaveChangesAsync();

            tenant.Slug = _slugService.GetSlug(tenant.Name, tenant.Id);

            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();

            return tenant.Slug;
        }

        public Task<List<Data.Entity.Models.Tenant>> ListAsync(string companySlug)
        {
            return _dbContext.Tenants
                .Include(item => item.Company)
                .Include(item => item.Logo)
                .Where(s => s.Company.Slug == companySlug.ToLower())
                .ToListAsync();
        }

        public Task<Data.Entity.Models.Tenant?> GetTenant(User user)
        {
            return _dbContext.Tenants.FirstOrDefaultAsync(item => item.OwnerId == user.Id);
        }

        public async Task<List<string?>> GetAllTenantByUser(User user, string companySlug, string? tenantSlug)
        {
            var tenants = _dbContext.Tenants
                                       .Include(item => item.Company)
                                       .Where(item => item.Company.Slug == companySlug.ToLower() && item.OwnerId == user.Id);

            if (!string.IsNullOrEmpty(tenantSlug))
            {
                tenants = tenants.Where(item => item.Slug == tenantSlug.ToLower());
            }

            return await tenants.Select(t => t.Slug).ToListAsync();
        }

        private async Task<Company> ValidateAsync(string companySlug)
        {
            var company =
                await _dbContext.Companies.FirstOrDefaultAsync(item => item.Slug == companySlug.ToLower());

            if (company is null)
            {
                throw new RecordNotFoundException($"Company {companySlug} not found");
            }

            //var isSlugDuplicate =
            //    await _dbContext.Tenants.AnyAsync(item => item.CompanyId == company.Id && item.Slug == slug.ToLower());
            //if (isSlugDuplicate)
            //{
            //    throw new InvalidActionException($"The name {slug} is already in use");
            //}

            return company;
        }

        private Data.Entity.Models.Tenant GetTenant(TenantModel model, Company company, User user)
        {
            var notificationType = model.NotificationTypes.Aggregate(model.NotificationTypes.First(), (current, next) => current | next);
            var paymentMethodType = model.PaymentMethodTypes.Aggregate(model.PaymentMethodTypes.First(), (current, next) => current | next);

            return new Data.Entity.Models.Tenant
            {
                CompanyId = company.Id,
                Slug = model.Name,
                OwnerId = user.Id,
                Name = model.Name,
                BusinessHours = GetBusinessHours(model.BusinessHours),
                PaymentMethodType = paymentMethodType,
                NotificationType = notificationType,
                Description = model.Description
            };
        }

        private List<BusinessHour> GetBusinessHours(List<BusinessHourModel> models)
        {
            return models.Select(item => new BusinessHour
            {
                DayOfWeek = item.DayOfWeek!.Value,
                IsClosed = item.IsClosed!.Value,
                OpeningTime = item.OpeningTime?.TimeOfDay,
                ClosingTime = item.ClosingTime?.TimeOfDay
            }).ToList();
        }
    }
}