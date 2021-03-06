﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Twilio
{
    internal class TwilioService : ITwilioService
    {
        private readonly IDbContext _dbContext;
        private readonly TwilioOptions _options;

        public TwilioService(IOptions<TwilioOptions> options, IDbContext dbContext)
        {
            _dbContext = dbContext;
            _options = options.Value;
        }

        public async Task SendAsync(int shortMessageId)
        {
            var shortMessage = await _dbContext.ShortMessages.FirstOrDefaultAsync(item => item.Id == shortMessageId);

            await SendAsync(shortMessage);
        }

        public async Task SendAsync(ShortMessage message)
        {
            if (message.SentAt != null)
            {
                return;
            }

            await SendCoreAsync(message);

            message.SentAt = DateTime.UtcNow;

            if (message.Id == default)
            {
                _dbContext.ShortMessages.Add(message);
            }
            else
            {
                _dbContext.ShortMessages.Update(message);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task SendCoreAsync(ShortMessage message)
        {
            var credentials = await GetCredentialsAsync(message.CompanyId);

            TwilioClient.Init(credentials.AccountSid, credentials.AuthToken);

            var from = GetPhoneNumber(credentials.PhoneNumber, message.IsWhatsApp).ToString();
            message.To = GetPhoneNumber(message.To, message.IsWhatsApp).ToString();

            try
            {
                var messageResource = await MessageResource.CreateAsync(body: message.Body, from: from, to: message.To);
            }
            // Invalid 'To' Phone Number
            // https://www.twilio.com/docs/api/errors/21211
            catch (ApiException apiException) when (apiException.Code == 21211)
            {
                throw new InvalidActionException($"The number {message.To} is not a valid phone number");
            }
        }

        private PhoneNumber GetPhoneNumber(string phoneNumber, bool isWhatsApp)
        {
            if (isWhatsApp)
            {
                return new PhoneNumber($"whatsapp:{phoneNumber}");
            }

            return new PhoneNumber(phoneNumber);
        }

        private async Task<(string AccountSid, string AuthToken, string PhoneNumber)> GetCredentialsAsync(int companyId)
        {
            var twilioAccount =
                await _dbContext.TwilioAccounts.FirstOrDefaultAsync(item => item.CompanyId == companyId);

            if (twilioAccount != null)
            {
                return (twilioAccount.AccountSid, twilioAccount.AuthToken, twilioAccount.PhoneNumber);
            }

            return (_options.AccountSid, _options.AuthToken, _options.PhoneNumber);
        }
    }
}