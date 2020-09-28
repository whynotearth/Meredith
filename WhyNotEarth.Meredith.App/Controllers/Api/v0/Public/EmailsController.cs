﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.SendGrid;
using WhyNotEarth.Meredith.Emails;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/emails")]
    [ProducesErrorResponseType(typeof(void))]
    public class EmailsController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public EmailsController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("sendgrid/webhook")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<OkResult> Create(List<SendGridEventItem> events)
        {
            foreach (var eventList in events.Batch(100))
            {
                foreach (var eventItem in eventList)
                {
                    await eventItem.Apply(_dbContext);
                }

                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }

        [Returns404]
        [HttpGet("{companySlug}/stats")]
        public async Task<ActionResult<EmailStatsResult>> Stats(string companySlug)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Name == companySlug.ToLower());

            if (company is null)
            {
                return NotFound($"Company '{companySlug}' not found.");
            }

            var lastMonth = DateTime.UtcNow.AddMonths(-1);
            var monthlySentEmails = await _dbContext.Emails.Where(item =>
                item.CreatedAt >= lastMonth &&
                item.CompanyId == company.Id).CountAsync();

            var monthlyActiveUsers = await _dbContext.Emails
                .Include(item => item.Events)
                .Where(item => item.Status >= EmailStatus.Opened && item.CreatedAt >= lastMonth)
                .Select(item => item.EmailAddress)
                .Distinct()
                .CountAsync();

            return Ok(new EmailStatsResult(monthlyActiveUsers, monthlySentEmails));
        }
    }

    public class SendGridEventItem
    {
        // Schema: https://sendgrid.com/docs/for-developers/tracking-events/event/
        [JsonProperty("EmailId")]
        public int? EmailId { get; set; }

        [JsonProperty(nameof(Emails.Email.CompanyId))]
        public int CompanyId { get; set; }

        [JsonProperty(nameof(Emails.Email.MemoId))]
        public int? MemoId { get; set; }

        [JsonProperty(nameof(Emails.Email.JumpStartId))]
        public int? JumpStartId { get; set; }

        [JsonProperty(nameof(Emails.Email.NewJumpStartId))]
        public int? NewJumpStartId { get; set; }

        public int Timestamp { get; set; }

        public string Email { get; set; } = null!;

        public string Event { get; set; } = null!;

        public EmailStatus Status =>
            Event switch
            {
                "delivered" => EmailStatus.Delivered,
                "open" => EmailStatus.Opened,
                "click" => EmailStatus.Clicked,
                _ => EmailStatus.None
            };

        public EmailEventType EventType =>
            Event switch
            {
                "delivered" => EmailEventType.Delivered,
                "open" => EmailEventType.Opened,
                "click" => EmailEventType.Clicked,
                _ => EmailEventType.None
            };

        public DateTime DateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).UtcDateTime;

        public async Task Apply(IDbContext dbContext)
        {
            Email email;

            if (EmailId.HasValue)
            {
                email = await dbContext.Emails.FirstOrDefaultAsync(item => item.Id == EmailId);
            }
            // TODO: Remove the else on the next version
            else
            {
                var query = dbContext.Emails.Where(item => item.CompanyId == CompanyId && item.EmailAddress == Email);

                if (MemoId.HasValue)
                {
                    query = query.Where(item => item.MemoId == MemoId);
                }
                else if (JumpStartId.HasValue)
                {
                    query = query.Where(item => item.JumpStartId == JumpStartId);
                }
                else if (NewJumpStartId.HasValue)
                {
                    query = query.Where(item => item.NewJumpStartId == NewJumpStartId);
                }
                else
                {
                    return;
                }

                email = await query.FirstOrDefaultAsync();

                if (email is null)
                {
                    return;
                }
            }

            Update(email);
        }

        private void Update(Email email)
        {
            if (EventType != EmailEventType.None)
            {
                email.Events ??= new List<EmailEvent>();

                email.Events.Add(new EmailEvent
                {
                    Type = EventType,
                    DateTime = DateTime
                });
            }

            if (email.Status < Status)
            {
                email.Status = Status;
            }
        }
    }
}