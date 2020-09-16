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
        private readonly TenantService _tenantService;

        public FormTemplateService(TenantService tenantService, IDbContext dbContext)
        {
            _tenantService = tenantService;
            _dbContext = dbContext;
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
    }
}