﻿using System;
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
        private readonly IDbContext _dbContext;
        private readonly FormNotifications _formNotifications;
        private readonly IFormSignatureFileService _formSignatureFileService;
        private readonly TenantService _tenantService;
        private readonly IUrlShortenerService _urlShortenerService;
        private readonly IUserService _userService;
        private readonly IFormTemplateService _formTemplateService;

        public FormAnswerService(IDbContext dbContext, TenantService tenantService,
            IFormSignatureFileService formSignatureFileService, FormNotifications formNotifications,
            IBackgroundJobClient backgroundJobClient, IClientService clientService, IUrlShortenerService urlShortenerService,
            IUserService userService, IFormTemplateService formTemplateService)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
            _formSignatureFileService = formSignatureFileService;
            _formNotifications = formNotifications;
            _backgroundJobClient = backgroundJobClient;
            _clientService = clientService;
            _urlShortenerService = urlShortenerService;
            _userService = userService;
            _formTemplateService = formTemplateService;
        }

        public async Task<byte[]> GetPngAsync(int formTemplateId, User user)
        {
            var formTemplate = await _formTemplateService.GetAsync(formTemplateId, user);

            return await _formSignatureFileService.GetPngAsync(formTemplate);
        }

        public async Task<byte[]> GetPdfAsync(int formTemplateId, User user)
        {
            var formTemplate = await _formTemplateService.GetAsync(formTemplateId, user);

            return await _formSignatureFileService.GetPdfAsync(formTemplate);
        }

        public async Task<byte[]> GetPngAsync(int formTemplateId, FormSignatureModel model, User user)
        {
            var formTemplate = await ValidateOwner(formTemplateId, user);

            var formSignature = Map(formTemplate, model, null, false);

            return await _formSignatureFileService.GetPngAsync(formSignature, user.FullName);
        }

        public async Task<byte[]> GetPngAsync(int formTemplateId, int clientId, FormSignatureModel model, User user)
        {
            var formTemplate = await _formTemplateService.GetAsync(formTemplateId, user);

            var client = await _dbContext.Clients
                .FirstOrDefaultAsync(item => item.Id == clientId);

            var formSignature = Map(formTemplate, model, clientId, false);

            return await _formSignatureFileService.GetPngAsync(formSignature, client.FullName);
        }

        public async Task SubmitAsync(int formTemplateId, int clientId, FormSignatureModel model, User user)
        {
            await _clientService.ValidateOwnerOrSelfAsync(clientId, user);

            await ValidateFormDuplicateSignatureAsync(formTemplateId, clientId);

            var formTemplate = await _formTemplateService.GetAsync(formTemplateId);

            var formSignature = Map(formTemplate, model, clientId, true);

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

            if (client.PhoneNumber is null)
            {
                throw new InvalidActionException("Client does not have a phone number");
            }

            var formUrl = await GetFormUrlAsync(callbackUrl, client.User);

            var shortMessage = _formNotifications.GetConsentNotification(client.Tenant, client, formUrl);

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

        private FormSignature Map(FormTemplate formTemplate, FormSignatureModel model, int? clientId, bool isSignatureImageRequired)
        {
            if (isSignatureImageRequired && model.SignatureImage is null)
            {
                throw new InvalidActionException("The signatureImage field is required.");
            }

            var answers = new List<FormAnswer>();

            foreach (var formAnswerModel in model.Answers)
            {
                var formItem = formTemplate.Items.FirstOrDefault(item => item.Id == formAnswerModel.FormItemId);

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
                SignatureImage = model.SignatureImage ?? string.Empty,
                NotificationCallBackUrl = model.NotificationCallBackUrl,
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task<string> GetFormUrlAsync(string callbackUrl, User user)
        {
            var token = await _userService.GenerateJwtTokenAsync(user);

            var url = UrlHelper.AddQueryString(callbackUrl, new Dictionary<string, string>
            {
                {"token", token}
            });

            var shortUrl = await _urlShortenerService.AddAsync(url);

            return shortUrl.Url;
        }

        private async Task<FormTemplate> ValidateOwner(int formTemplateId, User user)
        {
            var formTemplate = await _formTemplateService.GetAsync(formTemplateId);

            await _tenantService.CheckOwnerAsync(user, formTemplate.TenantId);

            return formTemplate;
        }
    }
}