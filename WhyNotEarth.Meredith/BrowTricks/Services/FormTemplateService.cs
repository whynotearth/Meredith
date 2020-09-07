using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private readonly TenantService _tenantService;

        public FormTemplateService(TenantService tenantService, IDbContext dbContext)
        {
            _tenantService = tenantService;
            _dbContext = dbContext;
        }

        public async Task CreateAsync(string tenantSlug, FormTemplateModel model, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            await ValidateAsync(model, tenant);

            var formTemplate = Map(new FormTemplate(), model, tenant.Id);
            formTemplate.CreatedAt = DateTime.UtcNow;

            _dbContext.FormTemplates.Add(formTemplate);
            await _dbContext.SaveChangesAsync();
        }


        public async Task EditAsync(string tenantSlug, int? formTemplateId, FormTemplateModel model, User user)
        {
            if (formTemplateId is null)
            {
                await CreateAsync(tenantSlug, model, user);
                return;
            }

            var formTemplate = await _dbContext.FormTemplates
                .FirstOrDefaultAsync(item => item.Id == formTemplateId);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, formTemplate.TenantId);

            formTemplate = Map(formTemplate, model, formTemplate.TenantId);

            _dbContext.FormTemplates.Update(formTemplate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<FormTemplate>> GetListAsync(string tenantSlug, User user)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            var formTemplates = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .Where(item => item.TenantId == tenant.Id)
                .ToListAsync();

            formTemplates = AddDefaults(tenant, formTemplates);

            return formTemplates;
        }

        public async Task DeleteAsync(int formTemplateId, User user)
        {
            var formTemplate = await _dbContext.FormTemplates.FirstOrDefaultAsync(item => item.Id == formTemplateId);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, formTemplate.TenantId);

            _dbContext.FormTemplates.Remove(formTemplate);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<FormTemplate> GetAsync(int formTemplateId, User user)
        {
            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, formTemplate.TenantId);

            return formTemplate;
        }

        public async Task<FormTemplate> GetAsync(Public.Tenant tenant, FormTemplateType type)
        {
            if (type < FormTemplateType.Disclosure || type > FormTemplateType.Cancellation)
            {
                throw new InvalidEnumArgumentException();
            }

            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.TenantId == tenant.Id && item.Type == type);

            if (formTemplate != null)
            {
                return formTemplate;
            }

            return GetDefault(tenant, type);
        }

        private List<FormTemplate> AddDefaults(Public.Tenant tenant, List<FormTemplate> formTemplates)
        {
            var types = new[] { FormTemplateType.Disclosure, FormTemplateType.Aftercare, FormTemplateType.Cancellation };

            foreach (var type in types)
            {
                if (formTemplates.Any(item => item.Type == type))
                {
                    continue;
                }

                var formTemplate = GetDefault(tenant, type);

                formTemplates.Add(formTemplate);
            }

            return formTemplates;
        }

        private FormTemplate GetDefault(Public.Tenant tenant, FormTemplateType type)
        {
            return type switch
            {
                FormTemplateType.Disclosure => GetDefaultDisclosure(tenant),
                FormTemplateType.Aftercare => GetDefaultAftercare(tenant),
                FormTemplateType.Cancellation => GetDefaultCancellation(tenant),
                _ => throw new NotSupportedException()
            };
        }

        private async Task ValidateAsync(FormTemplateModel model, Public.Tenant tenant)
        {
            if (model.Type == FormTemplateType.Custom)
            {
                return;
            }

            var hasThisType =
                await _dbContext.FormTemplates.AnyAsync(item => item.TenantId == tenant.Id && item.Type == model.Type);

            if (hasThisType)
            {
                throw new InvalidActionException("You already have a form template with this type");
            }
        }

        private FormTemplate Map(FormTemplate formTemplate, FormTemplateModel model, int tenantId)
        {
            formTemplate.TenantId = tenantId;
            formTemplate.Name = model.Name;
            formTemplate.Type = model.Type.Value;
            formTemplate.Items = model.Items.Select(item => Map(item)).ToList();

            return formTemplate;
        }

        private FormItem Map(FormItemModel model)
        {
            return new FormItem
            {
                Id = Guid.NewGuid(),
                Type = model.Type.Value,
                IsRequired = model.IsRequired.Value,
                Value = model.Value,
                Options = model.Options
            };
        }

        private FormTemplate GetDefaultDisclosure(Public.Tenant tenant)
        {
            return new FormTemplate
            {
                TenantId = tenant.Id,
                Tenant = tenant,
                Name = "PMU Disclosure Form",
                Type = FormTemplateType.Disclosure,
                Items = new List<FormItem>
                {
                    new FormItem
                    {
                        Id = new Guid("35e0b105-821f-4952-b915-390238a87dc0"),
                        Type = FormItemType.Text,
                        IsRequired = false,
                        Value = "Hello!"
                    }
                }
            };
        }

        private FormTemplate GetDefaultAftercare(Public.Tenant tenant)
        {
            return new FormTemplate
            {
                TenantId = tenant.Id,
                Tenant = tenant,
                Name = "Aftercare instructions agreement",
                Type = FormTemplateType.Disclosure,
                Items = new List<FormItem>
                {
                    new FormItem
                    {
                        Id = new Guid("93835fa8-87de-47b1-951b-3d6457f78419"),
                        Type = FormItemType.Text,
                        IsRequired = false,
                        Value = "Hello!"
                    }
                }
            };
        }

        private FormTemplate GetDefaultCancellation(Public.Tenant tenant)
        {
            return new FormTemplate
            {
                TenantId = tenant.Id,
                Tenant = tenant,
                Name = "Cancellation Policy Agreement",
                Type = FormTemplateType.Disclosure,
                Items = new List<FormItem>
                {
                    new FormItem
                    {
                        Id = new Guid("1c536098-57b4-4da0-8ed7-f65014448cfb"),
                        Type = FormItemType.Text,
                        IsRequired = false,
                        Value = "Hello!"
                    }
                }
            };
        }
    }
}