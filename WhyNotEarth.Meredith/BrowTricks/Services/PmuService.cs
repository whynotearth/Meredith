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
    internal class PmuService : IPmuService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IClientService _clientService;
        private readonly IDbContext _dbContext;
        private readonly IFormTemplateService _formTemplateService;
        private readonly ILoginTokenService _loginTokenService;
        private readonly PmuNotifications _pmuNotifications;
        private readonly IPmuPdfService _pmuPdfService;
        private readonly TenantService _tenantService;

        public PmuService(IDbContext dbContext, TenantService tenantService, IPmuPdfService pmuPdfService,
            PmuNotifications pmuNotifications, IBackgroundJobClient backgroundJobClient,
            ILoginTokenService loginTokenService, IClientService clientService,
            IFormTemplateService formTemplateService)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
            _pmuPdfService = pmuPdfService;
            _pmuNotifications = pmuNotifications;
            _backgroundJobClient = backgroundJobClient;
            _loginTokenService = loginTokenService;
            _clientService = clientService;
            _formTemplateService = formTemplateService;
        }

        public async Task<byte[]> GetPngAsync(string tenantSlug, User user)
        {
            var tenant = await _clientService.ValidateOwnerOrClient(tenantSlug, user);

            return await _pmuPdfService.GetPngAsync(tenant);
        }

        public async Task<byte[]> GetPngAsync(int clientId, User user)
        {
            var client = await _clientService.ValidateOwnerOrSelf(clientId, user);

            return await _pmuPdfService.GetPngAsync(client.Tenant);
        }

        public async Task SignAsync(int clientId, PmuSignModel model, User user)
        {
            var client = await _clientService.ValidateOwnerOrSelf(clientId, user);

            await ValidateAsync(clientId);

            var formTemplate = await _formTemplateService.GetAsync(client.Tenant, FormTemplateType.Disclosure);

            var answers = Map(formTemplate, model);

            var formSignature = new FormSignature
            {
                Name = formTemplate.Name,
                Type = FormTemplateType.Disclosure,
                CreatedAt = DateTime.UtcNow,
                ClientId = clientId,
                Answers = answers
            };

            _dbContext.FormSignatures.Add(formSignature);
            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<IClientSaveSignatureJob>(service =>
                service.SaveSignature(formSignature.Id));
        }

        public async Task SendConsentNotificationAsync(int clientId, User user, string callbackUrl)
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

            await ValidateAsync(clientId);

            var formUrl = await GetFormUrlAsync(callbackUrl, client.User);

            var shortMessage = _pmuNotifications.GetConsentNotification(client.Tenant, client.User, formUrl);

            _dbContext.ShortMessages.Add(shortMessage);
            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Enqueue<ITwilioService>(service =>
                service.SendAsync(shortMessage.Id));
        }

        private async Task ValidateAsync(int clientId)
        {
            var isSignedPmu = await _dbContext.FormSignatures.AnyAsync(item =>
                item.ClientId == clientId && item.Type == FormTemplateType.Disclosure);

            if (isSignedPmu)
            {
                throw new InvalidActionException("This client is already signed their PMU form");
            }
        }

        private List<FormAnswer> Map(FormTemplate formTemplate, PmuSignModel model)
        {
            var result = new List<FormAnswer>();

            foreach (var formAnswerModel in model.Answers)
            {
                var formItem = formTemplate.Items.FirstOrDefault(item => item.Id == formAnswerModel.FormItemId);

                if (formItem is null)
                {
                    throw new InvalidActionException($"Invalid form item: {formAnswerModel.FormItemId}");
                }
            }

            foreach (var formItem in formTemplate.Items)
            {
                var answer = model.Answers.FirstOrDefault(item => item.FormItemId == formItem.Id);

                if (answer is null)
                {
                    throw new InvalidActionException($"Missing form item: {formItem.Id}");
                }

                result.Add(new FormAnswer
                {
                    Type = formItem.Type,
                    Question = formItem.Value,
                    IsRequired = formItem.IsRequired,
                    Options = formItem.Options,
                    Answers = answer.Value
                });
            }

            return result;
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
    }
}