using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
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
            var tenant = await ValidateAsync(model, user, tenantSlug, null);

            var formTemplate = Map(new FormTemplate(), model, tenant.Id);

            _dbContext.FormTemplates.Add(formTemplate);
            await _dbContext.SaveChangesAsync();

            return formTemplate.Id;
        }

        public async Task EditAsync(string tenantSlug, int formTemplateId, FormTemplateModel model, User user)
        {
            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            await ValidateAsync(model, user, tenantSlug, formTemplate.Id);

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

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"{type} form template not found");
            }

            return formTemplate;
        }

        private async Task<Public.Tenant> ValidateAsync(FormTemplateModel model, User user, string tenantSlug,
            int? formTemplateId)
        {
            var tenant = await _tenantService.CheckOwnerAsync(user, tenantSlug);

            if (model.Type == FormTemplateType.Custom)
            {
                return tenant;
            }

            Expression<Func<FormTemplate, bool>> query;

            if (formTemplateId.HasValue)
            {
                query = item => item.Id != formTemplateId && item.TenantId == tenant.Id && item.Type == model.Type;
            }
            else
            {
                query = item => item.TenantId == tenant.Id && item.Type == model.Type;
            }

            var hasThisType = await _dbContext.FormTemplates.AnyAsync(query);

            if (hasThisType)
            {
                throw new InvalidActionException("You already have a form template with this type");
            }

            return tenant;
        }

        private FormTemplate Map(FormTemplate formTemplate, FormTemplateModel model, int tenantId)
        {
            formTemplate.TenantId = tenantId;
            formTemplate.Name = model.Name;
            formTemplate.Type = model.Type.Value;
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