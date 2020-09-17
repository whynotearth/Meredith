using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Jobs;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.HelloSign;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;
using WhyNotEarth.Meredith.Twilio;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    internal class FormAnswerService : IFormAnswerService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IClientService _clientService;
        private readonly IDbContext _dbContext;
        private readonly FormNotifications _formNotifications;
        private readonly IFormSignatureFileService _formSignatureFileService;
        private readonly ILoginTokenService _loginTokenService;
        private readonly TenantService _tenantService;

        public FormAnswerService(IDbContext dbContext, TenantService tenantService,
            IFormSignatureFileService formSignatureFileService,
            FormNotifications formNotifications, IBackgroundJobClient backgroundJobClient,
            ILoginTokenService loginTokenService, IClientService clientService)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
            _formSignatureFileService = formSignatureFileService;
            _formNotifications = formNotifications;
            _backgroundJobClient = backgroundJobClient;
            _loginTokenService = loginTokenService;
            _clientService = clientService;
        }

        public async Task<byte[]> GetPngAsync(int formTemplateId, User user)
        {
            var formTemplate = await ValidateOwnerOrClient(formTemplateId, user);

            return await _formSignatureFileService.GetPngAsync(formTemplate);
        }

        public async Task<byte[]> GetPngAsync(int formTemplateId, int clientId, PmuSignModel model, User user)
        {
            var formTemplate = await ValidateOwnerOrClient(formTemplateId, user);

            var client = await _dbContext.Clients
                .Include(item => item.User)
                .FirstOrDefaultAsync(item => item.Id == clientId);

            var formSignature = Map(formTemplate, model, clientId, client);

            return await _formSignatureFileService.GetPngAsync(formSignature);
        }

        public async Task SubmitAsync(int formTemplateId, int clientId, PmuSignModel model, User user)
        {
            await _clientService.ValidateOwnerOrSelf(clientId, user);

            await ValidateFormDuplicateSignatureAsync(formTemplateId, clientId);

            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            var formSignature = Map(formTemplate, model, clientId, null);

            _dbContext.FormSignatures.Add(formSignature);
            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<IClientSaveSignatureJob>(service =>
                service.SaveSignature(formSignature.Id));
        }

        public async Task SendNotificationAsync(int formTemplateId, int clientId, User user, string callbackUrl)
        {
            var client = await _dbContext.Clients
                .Include(item => item.User)
                .Include(item => item.Tenant)
                .FirstOrDefaultAsync(item => item.Id == clientId);

            if (client is null)
            {
                throw new RecordNotFoundException($"client {clientId} not found");
            }

            await _tenantService.CheckOwnerAsync(user, client.TenantId);

            await ValidateFormDuplicateSignatureAsync(formTemplateId, clientId);

            var formUrl = await GetFormUrlAsync(callbackUrl, client.User);

            var shortMessage = _formNotifications.GetConsentNotification(client.Tenant, client.User, formUrl);

            _dbContext.ShortMessages.Add(shortMessage);
            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<ITwilioService>(service =>
                service.SendAsync(shortMessage.Id));
        }

        private async Task ValidateFormDuplicateSignatureAsync(int formTemplateId, int clientId)
        {
            var isFormSigned = await _dbContext.FormSignatures.AnyAsync(item =>
                item.FormTemplateId == formTemplateId && item.ClientId == clientId);

            if (isFormSigned)
            {
                throw new InvalidActionException("You've already signed this form");
            }
        }

        private FormSignature Map(FormTemplate formTemplate, PmuSignModel model, int clientId, Client? client)
        {
            var answers = new List<FormAnswer>();

            foreach (var formAnswerModel in model.Answers)
            {
                var formItem = formTemplate.Items?.FirstOrDefault(item => item.Id == formAnswerModel.FormItemId);

                if (formItem is null)
                {
                    throw new InvalidActionException($"Invalid form item: {formAnswerModel.FormItemId}");
                }
            }

            if (formTemplate.Items != null)
            {
                foreach (var formItem in formTemplate.Items)
                {
                    var answer = model.Answers.FirstOrDefault(item => item.FormItemId == formItem.Id);

                    if (answer is null)
                    {
                        if (formItem.IsRequired)
                        {
                            throw new InvalidActionException($"Missing answer for form item: {formItem.Id}");
                        }

                        continue;
                    }

                    answers.Add(new FormAnswer
                    {
                        Type = formItem.Type,
                        Question = formItem.Value,
                        IsRequired = formItem.IsRequired,
                        Options = formItem.Options,
                        Answers = answer.Value
                    });
                }
            }

            return new FormSignature
            {
                FormTemplateId = formTemplate.Id,
                ClientId = clientId,
                Client = client!,
                Name = formTemplate.Name,
                Answers = answers,
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task<string> GetFormUrlAsync(string callbackUrl, User user)
        {
            var token = await _loginTokenService.GenerateTokenAsync(user);

            var finalUrl = UrlHelper.AddQueryString(callbackUrl, new Dictionary<string, string>
            {
                {"token", token}
            });

            return finalUrl;
        }

        private async Task<FormTemplate> ValidateOwnerOrClient(int formTemplateId, User user)
        {
            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            await _clientService.ValidateOwnerOrClient(formTemplate.TenantId, user);

            return formTemplate;
        }

        private async Task<FormSignature> ValidateOwnerOrClient(int formTemplateId, int clientId, User user)
        {
            var formSignature = await _dbContext.FormSignatures
                .Include(item => item.FormTemplate)
                .Include(item => item.Answers)
                .FirstOrDefaultAsync(item => item.FormTemplateId == formTemplateId && item.ClientId == clientId);

            if (formSignature is null)
            {
                throw new RecordNotFoundException(
                    $"The form template {formTemplateId} for client {clientId} not found");
            }

            await _clientService.ValidateOwnerOrClient(formSignature.FormTemplate.TenantId, user);

            return formSignature;
        }
    }
}