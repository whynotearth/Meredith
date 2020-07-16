using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant.Models;

namespace WhyNotEarth.Meredith.Tenant
{
    public class TenantService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly SlugService _slugService;
        private readonly IUserService _userService;

        public TenantService(MeredithDbContext dbContext, SlugService slugService, IUserService userService)
        {
            _dbContext = dbContext;
            _slugService = slugService;
            _userService = userService;
        }

        public async Task<string> CreateAsync(string companySlug, TenantCreateModel model, User user)
        {
            var company = await ValidateAsync(companySlug, user);

            var tenant = Map(model, company, user);

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
            return _dbContext.Tenants.FirstOrDefaultAsync(item => item.OwnerId == user.Id)!;
        }

        public async Task<List<Data.Entity.Models.Tenant>> GetAllTenantsByUser(User user, string companySlug)
        {
            var tenants = await _dbContext.Tenants
                .Include(item => item.Company)
                .Include(item => item.Logo)
                .Where(item => item.Company.Slug == companySlug.ToLower() && item.OwnerId == user.Id)
                .ToListAsync();

            return tenants;
        }

        private async Task<Company> ValidateAsync(string companySlug, User user)
        {
            var company =
                await _dbContext.Companies.FirstOrDefaultAsync(item => item.Slug == companySlug.ToLower());

            if (company is null)
            {
                throw new RecordNotFoundException($"Company {companySlug} not found");
            }

            var isExternalAccountConnected = await _userService.IsExternalAccountConnected(user);
            if (!isExternalAccountConnected)
            {
                throw new InvalidActionException("You need to connect your Google or Facebook account");
            }

            return company;
        }

        private Data.Entity.Models.Tenant Map(TenantCreateModel model, Company company, User user)
        {
            var result = new Data.Entity.Models.Tenant
            {
                CompanyId = company.Id,
                Slug = model.Name,
                OwnerId = user.Id,
                Name = model.Name,
                BusinessHours = GetBusinessHours(model.BusinessHours),
                PaymentMethodType = model.PaymentMethodTypes.ToFlag(),
                NotificationType = model.NotificationTypes.ToFlag(),
                Description = model.Description,
                Tags = model.Tags,
                DeliveryTime = model.DeliveryTime ?? default,
                DeliveryFee = model.DeliveryFee ?? default,
                IsActive = true,
                PhoneNumber = model.PhoneNumber,
                FacebookUrl = model.FacebookUrl,
                WhatsAppNumber = model.WhatsAppNumber
            };

            if (model.LogoUrl != null)
            {
                result.Logo = new TenantImage
                {
                    Url = model.LogoUrl
                };
            }

            return result;
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

        public Task<Data.Entity.Models.Tenant> GeAsync(string tenantSlug)
        {
            return _dbContext.Tenants
                .Include(item => item.Logo)
                .Include(item => item.BusinessHours)
                .Include(item => item.Address)
                .FirstOrDefaultAsync(item => item.Slug == tenantSlug);
        }

        public async Task<Data.Entity.Models.Tenant> CheckPermissionAsync(User user, string tenantSlug)
        {
            var tenant =
                await _dbContext.Tenants.FirstOrDefaultAsync(item =>
                    item.Slug == tenantSlug && item.OwnerId == user.Id);

            if (tenant is null)
            {
                tenant = await _dbContext.Tenants.FirstOrDefaultAsync(item => item.Slug == tenantSlug);
                if (tenant is null)
                {
                    throw new RecordNotFoundException($"Tenant {tenantSlug} not found");
                }

                throw new ForbiddenException("You don't own this tenant");
            }

            return tenant;
        }

        public async Task<Data.Entity.Models.Tenant> CheckPermissionAsync(User user, int tenantId)
        {
            var tenant =
                await _dbContext.Tenants.FirstOrDefaultAsync(item => item.Id == tenantId && item.OwnerId == user.Id);

            if (tenant is null)
            {
                tenant = await _dbContext.Tenants.FirstOrDefaultAsync(item => item.Id == tenantId);
                if (tenant is null)
                {
                    throw new RecordNotFoundException($"Tenant {tenantId} not found");
                }

                throw new ForbiddenException("You don't own this tenant");
            }

            return tenant;
        }

        public async Task<Address?> GetAddressAsync(string tenantSlug)
        {
            var tenant = await _dbContext.Tenants
                .Include(item => item.Address)
                .FirstOrDefaultAsync(item => item.Slug == tenantSlug);

            if (tenant is null)
            {
                throw new RecordNotFoundException($"Tenant {tenantSlug} not found");
            }

            return tenant.Address;
        }

        public async Task SetAddressAsync(string tenantSlug, AddressModel model, User user)
        {
            var tenant = await CheckPermissionAsync(user, tenantSlug);

            tenant.Address ??= new Address();

            tenant.Address.Street = model.Street;
            tenant.Address.ApartmentNumber = model.ApartmentNumber;
            tenant.Address.City = model.City;
            tenant.Address.ZipCode = model.ZipCode;
            tenant.Address.State = model.State;

            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task SetActivityAsync(string tenantSlug, TenantActivityModel model, User user)
        {
            var tenant = await CheckPermissionAsync(user, tenantSlug);

            tenant.IsActive = model.IsActive!.Value;

            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(string tenantSlug, TenantEditModel model, User user)
        {
            var tenant = await CheckPermissionAsync(user, tenantSlug);

            tenant = Map(tenant, model);

            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
        }

        private Data.Entity.Models.Tenant Map(Data.Entity.Models.Tenant tenant, TenantEditModel model)
        {
            if (model.HasPromotion.HasValue)
            {
                tenant.HasPromotion = model.HasPromotion.Value;
                tenant.PromotionPercent = model.PromotionPercent!.Value;
            }

            return tenant;
        }
    }
}