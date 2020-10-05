using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Shop;
using WhyNotEarth.Meredith.Tenant.Models;

namespace WhyNotEarth.Meredith.Tenant
{
    public class TenantService
    {
        private readonly IDbContext _dbContext;
        private readonly SlugService _slugService;
        private readonly ICloudinaryService _cloudinaryService;

        public TenantService(IDbContext dbContext, SlugService slugService, ICloudinaryService cloudinaryService)
        {
            _dbContext = dbContext;
            _slugService = slugService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<string> CreateAsync(string companySlug, TenantCreateModel model, User user)
        {
            var company = await ValidateAsync(companySlug, user);

            var tenant = Map(model, company, user);

            if (tenant.Logo != null)
            {
                var image = _cloudinaryService.GetUpdatedImageParameters(new Image()
                {
                    Height = tenant.Logo.Height,
                    Width = tenant.Logo.Width,
                    Order = tenant.Logo.Order,
                    Title = tenant.Logo.Title,
                    AltText = tenant.Logo.AltText,
                    Url = tenant.Logo.Url,
                    FileSize = tenant.Logo.FileSize,
                    CloudinaryPublicId = tenant.Logo.CloudinaryPublicId
                });

                tenant.Logo = new TenantImage(image);
            }

            _dbContext.Tenants.Add(tenant);
            await _dbContext.SaveChangesAsync();

            tenant.Slug = _slugService.GetSlug(tenant.Name, tenant.Id);

            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();

            return tenant.Slug;
        }

        public Task<List<Public.Tenant>> ListAsync(string companySlug)
        {
            return _dbContext.Tenants
                .Include(item => item.Company)
                .Include(item => item.Logo)
                .Where(s => s.Company.Slug == companySlug.ToLower())
                .ToListAsync();
        }

        public Task<Public.Tenant?> GetTenant(User user)
        {
            return _dbContext.Tenants.FirstOrDefaultAsync(item => item.OwnerId == user.Id)!;
        }

        public async Task<List<Public.Tenant>> GetAllTenantsByUser(User user, string companySlug)
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

            if (!user.EmailConfirmed && !user.PhoneNumberConfirmed)
            {
                throw new InvalidActionException("You need to confirm your email address or phone number");
            }

            return company;
        }

        private Public.Tenant Map(TenantCreateModel model, Company company, User user)
        {
            var result = new Public.Tenant
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

        private List<BusinessHour> GetBusinessHours(List<BusinessHourModel>? models)
        {
            if (models is null)
            {
                return new List<BusinessHour>();
            }

            return models.Select(item => new BusinessHour
            {
                DayOfWeek = item.DayOfWeek!.Value,
                IsClosed = item.IsClosed!.Value,
                OpeningTime = item.OpeningTime?.TimeOfDay,
                ClosingTime = item.ClosingTime?.TimeOfDay
            }).ToList();
        }

        public Task<Public.Tenant> GeAsync(string tenantSlug)
        {
            return _dbContext.Tenants
                .Include(item => item.Logo)
                .Include(item => item.BusinessHours)
                .Include(item => item.Address)
                .Include(item => item.Owner)
                .Include(item => item.Company)
                .FirstOrDefaultAsync(item => item.Slug == tenantSlug);
        }

        public async Task<Public.Tenant> CheckOwnerAsync(User owner, string tenantSlug)
        {
            var tenant =
                await _dbContext.Tenants.FirstOrDefaultAsync(item =>
                    item.Slug == tenantSlug && item.OwnerId == owner.Id);

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

        public async Task<Public.Tenant> CheckOwnerAsync(User owner, int tenantId)
        {
            var tenant =
                await _dbContext.Tenants.FirstOrDefaultAsync(item => item.Id == tenantId && item.OwnerId == owner.Id);

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
            var tenant = await CheckOwnerAsync(user, tenantSlug);

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
            var tenant = await CheckOwnerAsync(user, tenantSlug);

            tenant.IsActive = model.IsActive!.Value;

            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task EditAsync(string tenantSlug, TenantEditModel model, User user)
        {
            var tenant = await CheckOwnerAsync(user, tenantSlug);

            // Delete old profile image if already exists and is different than the updated one.
            if (tenant.Logo != null && tenant.Logo.CloudinaryPublicId != model.Logo.CloudinaryPublicId)
            {
                await _cloudinaryService.DeleteByUrlAsync(tenant.Logo.Url);
            }

            if (tenant.Logo != null)
            {
                var image = _cloudinaryService.GetUpdatedImageParameters(new Image()
                {
                    Height = tenant.Logo.Height,
                    Width = tenant.Logo.Width,
                    Order = tenant.Logo.Order,
                    Title = tenant.Logo.Title,
                    AltText = tenant.Logo.AltText,
                    Url = tenant.Logo.Url,
                    FileSize = tenant.Logo.FileSize,
                    CloudinaryPublicId = tenant.Logo.CloudinaryPublicId
                });

                tenant.Logo = new TenantImage(image);
            }
            
            tenant = Map(tenant, model);
            
            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
        }
        
       private Public.Tenant Map(Public.Tenant tenant, TenantEditModel model)
        {
            if (model.Name != null)
            {
                tenant.Name = model.Name;
            }

            if (model.Tags != null)
            {
                tenant.Tags = model.Tags;
            }

            if (model.Description != null)
            {
                tenant.Description = model.Description;
            }

            if (model.InstagramUrl != null)
            {
                tenant.InstagramUrl = model.InstagramUrl;
            }

            if (model.FacebookUrl != null)
            {
                tenant.FacebookUrl = model.FacebookUrl;
            }

            if (model.YouTubeUrl != null)
            {
                tenant.YouTubeUrl = model.YouTubeUrl;
            }

            if (model.WhatsAppNumber != null)
            {
                tenant.WhatsAppNumber = model.WhatsAppNumber;
            }

            if (model.HasPromotion.HasValue)
            {
                tenant.HasPromotion = model.HasPromotion.Value;
                tenant.PromotionPercent = model.PromotionPercent!.Value;
            }

            if (model.Logo != null)
            {
                if (tenant.Logo.CloudinaryPublicId != model.Logo.CloudinaryPublicId)
                {
                    tenant.Logo = new TenantImage()
                    {
                        Height = model.Logo.Height,
                        Width = model.Logo.Width,
                        Order = model.Logo.Order,
                        Title = model.Logo.Title,
                        Url = model.Logo.Url,
                        AltText = model.Logo.AltText,
                        FileSize = model.Logo.FileSize,
                        CloudinaryPublicId = model.Logo.CloudinaryPublicId,
                    };
                }
            }
            else
            {
                tenant.Logo = null;
            }
            
            return tenant;
        }
    }
}