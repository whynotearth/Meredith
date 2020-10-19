using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    internal class FormTemplateService : IFormTemplateService
    {
        private readonly IDbContext _dbContext;
        private readonly IClientService _clientService;
        private readonly TenantService _tenantService;

        public FormTemplateService(TenantService tenantService, IDbContext dbContext, IClientService clientService)
        {
            _tenantService = tenantService;
            _dbContext = dbContext;
            _clientService = clientService;
        }

        public async Task CreateDefaultsAsync(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var formTemplates = GetDefaultTemplates(tenant.Id);

            _dbContext.FormTemplates.AddRange(formTemplates);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CreateAsync(string tenantSlug, FormTemplateModel model, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var formTemplate = Map(new FormTemplate(), model, tenant.Id);

            _dbContext.FormTemplates.Add(formTemplate);
            await _dbContext.SaveChangesAsync();

            return formTemplate.Id;
        }

        public async Task EditAsync(string tenantSlug, int formTemplateId, FormTemplateModel model, User user)
        {
            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId && item.IsDeleted == false);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, tenantSlug);

            formTemplate = Map(formTemplate, model, formTemplate.TenantId);

            _dbContext.FormTemplates.Update(formTemplate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<FormTemplate>> GetListAsync(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var formTemplates = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .Where(item => item.TenantId == tenant.Id && item.IsDeleted == false)
                .ToListAsync();

            return formTemplates;
        }

        public async Task DeleteAsync(int formTemplateId, User user)
        {
            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, formTemplate.TenantId);

            formTemplate.IsDeleted = true;

            _dbContext.FormTemplates.Update(formTemplate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<FormTemplate> GetAsync(int formTemplateId, User user)
        {
            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId && item.IsDeleted == false);

            await _clientService.ValidateOwnerOrClientAsync(formTemplate.TenantId, user);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, formTemplate.TenantId);

            return formTemplate;
        }

        private FormTemplate Map(FormTemplate formTemplate, FormTemplateModel model, int tenantId)
        {
            formTemplate.TenantId = tenantId;
            formTemplate.Name = model.Name;
            formTemplate.Items = model.Items?.Select(Map).ToList();
            formTemplate.CreatedAt ??= DateTime.UtcNow;

            return formTemplate;
        }

        private FormItem Map(FormItemModel model)
        {
            return new FormItem
            {
                Type = model.Type.Value,
                IsRequired = model.IsRequired.Value,
                Value = model.Value,
                Options = model.Options
            };
        }

        private List<FormTemplate> GetDefaultTemplates(int tenantId)
        {
            return new List<FormTemplate>
            {
                new FormTemplate
                {
                    Name = "Pre and Post Care Agreement",
                    CreatedAt = DateTime.UtcNow,
                    TenantId = tenantId,
                    Items = new List<FormItem>
                    {
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.Pdf,
                            Value =
                                "I have read, or had read to me, the above Pre and Post Care instructions and expectations. I agree to follow the above directions and understand that how I heal depends on how closely I follow said directions. I understand a touchup appointment will be needed to complete the process.",
                            Options = new List<string>
                            {
                                "https://res.cloudinary.com/whynotearth/image/upload/v1602252454/BrowTricks/backend/Pre_and_Post_Care_Agreement_djswql.pdf"
                            }
                        }
                    }
                },
                new FormTemplate
                {
                    Name = "Booking Fee and Cancellation Policy",
                    CreatedAt = DateTime.UtcNow,
                    TenantId = tenantId,
                    Items = new List<FormItem>
                    {
                        new FormItem
                        {
                            IsRequired = true,
                            Type = FormItemType.AgreementRequest,
                            Value = @"Upon booking your first appointment, a $50 non- refundable deposit is charged to secure your appointment. The deposit will be deducted from the total cost of your initial procedure; the remaining balance will be due on your appointment day.  Your booking fee is non refundable, non transferable, and can only be applied to the scheduled service for the date and time secured. Please be sure about your appointment day and time before booking. 

If you need to reschedule, you may do so with Cancellation fee within 48 hours of your appointment.  A new booking fee will be charged for the new appointment date.  If you are unable to make your appointment, you MUST give a 48 hour notice to avoid a Cancellation Fee.  In this instance, 20% of the appointment is charged.  This pays for the time that was set aside for you and your appointment.  Your Booking Fee in this case is applied to the 25% charge, and a new Booking Fee must be paid to reschedule.  
All lost booking fees and cancellation fees may be waived at the discretion of owner on a case by case basis. 

I fully understand the Booking Fee and Cancellation Fee Policy, and hereby forfeit my Booking Fee to secure my appointment spot. 
I will also give 48 hour notice or pay 20% for my appointment reservation."
                        }
                    }
                }
            };
        }
    }
}