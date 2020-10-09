using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Jobs;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tenant;
using WhyNotEarth.Meredith.Twilio;
using WhyNotEarth.Meredith.UrlShortener;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    internal class FormAnswerService : IFormAnswerService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IClientService _clientService;
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly IDbContext _dbContext;
        private readonly FormNotifications _formNotifications;
        private readonly IFormSignatureFileService _formSignatureFileService;
        private readonly ILoginTokenService _loginTokenService;
        private readonly TenantService _tenantService;

        public FormAnswerService(IDbContext dbContext, TenantService tenantService,
            IFormSignatureFileService formSignatureFileService,
            FormNotifications formNotifications, IBackgroundJobClient backgroundJobClient,
            ILoginTokenService loginTokenService, IClientService clientService, IUrlShortenerService urlShortenerService)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
            _formSignatureFileService = formSignatureFileService;
            _formNotifications = formNotifications;
            _backgroundJobClient = backgroundJobClient;
            _loginTokenService = loginTokenService;
            _clientService = clientService;
            _urlShortenerService = urlShortenerService;
        }

        public async Task<byte[]> GetPngAsync(int formTemplateId, User user)
        {
            var formTemplate = await ValidateOwnerOrClient(formTemplateId, user);

            return await _formSignatureFileService.GetPngAsync(formTemplate);
        }

        public async Task<byte[]> GetPdfAsync(int formTemplateId, User user)
        {
            var formTemplate = await ValidateOwnerOrClient(formTemplateId, user);

            return await _formSignatureFileService.GetPdfAsync(formTemplate);
        }

        public async Task<byte[]> GetPngAsync(int formTemplateId, FormSignatureModel model, User user)
        {
            var formTemplate = await ValidateOwner(formTemplateId, user);

            var formSignature = Map(formTemplate, model, null);

            return await _formSignatureFileService.GetPngAsync(formSignature, user);
        }

        public async Task<byte[]> GetPngAsync(int formTemplateId, int clientId, FormSignatureModel model, User user)
        {
            var formTemplate = await ValidateOwnerOrClient(formTemplateId, user);

            var client = await _dbContext.Clients
                .Include(item => item.User)
                .FirstOrDefaultAsync(item => item.Id == clientId);

            var formSignature = Map(formTemplate, model, clientId);

            return await _formSignatureFileService.GetPngAsync(formSignature, client.User);
        }

        public async Task SubmitAsync(int formTemplateId, int clientId, FormSignatureModel model, User user)
        {
            await _clientService.ValidateOwnerOrSelfAsync(clientId, user);

            await ValidateFormDuplicateSignatureAsync(formTemplateId, clientId);

            var formTemplate = await _dbContext.FormTemplates
                .Include(item => item.Items)
                .FirstOrDefaultAsync(item => item.Id == formTemplateId);

            if (formTemplate is null)
            {
                throw new RecordNotFoundException($"Form template {formTemplateId} not found");
            }

            var formSignature = Map(formTemplate, model, clientId);

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

        private FormSignature Map(FormTemplate formTemplate, FormSignatureModel model, int? clientId)
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
                ClientId = clientId ?? default,
                Name = formTemplate.Name,
                Answers = answers,
                NotificationCallBackUrl = model.NotificationCallBackUrl,
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task<string> GetFormUrlAsync(string callbackUrl, User user)
        {
            var token = await _loginTokenService.GenerateTokenAsync(user);

            var url = UrlHelper.AddQueryString(callbackUrl, new Dictionary<string, string>
            {
                {"token", token}
            });

            var shortUrl = await _urlShortenerService.AddAsync(url);

            return shortUrl.Url;
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

            await _clientService.ValidateOwnerOrClientAsync(formTemplate.TenantId, user);

            return formTemplate;
        }

        private async Task<FormTemplate> ValidateOwner(int formTemplateId, User user)
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
    }
}