using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.SendGrid;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/sendgrid")]
    [ProducesErrorResponseType(typeof(void))]
    public class SendGridController : ControllerBase
    {
        private readonly MeredithDbContext _dbContext;

        public SendGridController(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("webhook")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<OkResult> Create(List<EventItem> events)
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

        [HttpGet("{companySlug}/emailstats")]
        public async Task<ActionResult<StatResult>> EmailStats(string companySlug)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Name == companySlug);
            if(company is null)
            {
                return NotFound($"Company '{companySlug}' not found.");
            }
            var lastMonth = DateTime.Now.AddMonths(-1);
            var monthlySentEmails = await _dbContext.EmailRecipients.Where(item =>
                item.CompanyId == company.Id).CountAsync();

            var monthlyActiveUsers = await _dbContext.EmailRecipients
                .Where(item =>
                    item.Status == EmailStatus.Opened &&
                    item.DeliverDateTime >= lastMonth)
                .Select(item => item.Email)
                .Distinct()
                .CountAsync();

            return Ok(new StatResult { MonthlyActiveUsers = monthlyActiveUsers, MonthlySentEmails = monthlySentEmails });
        }

        public class EventItem
        {
            // Schema: https://sendgrid.com/docs/for-developers/tracking-events/event/

            [JsonProperty(nameof(EmailRecipient.MemoId))]
            public int? MemoId { get; set; }

            [JsonProperty(nameof(EmailRecipient.JumpStartId))]
            public int? JumpStartId { get; set; }

            [JsonProperty(nameof(EmailRecipient.CompanyId))]
            public int CompanyId { get; set; }

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

            public DateTime DateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).UtcDateTime;

            public async Task Apply(MeredithDbContext dbContext)
            {
                EmailRecipient? emailRecipient = null;

                if (MemoId.HasValue)
                {
                    emailRecipient = await dbContext.EmailRecipients.FirstOrDefaultAsync(item =>
                        item.MemoId == MemoId && item.Email == Email);
                }
                else if (JumpStartId.HasValue)
                {
                    emailRecipient = await dbContext.EmailRecipients.FirstOrDefaultAsync(item =>
                        item.JumpStartId == JumpStartId && item.Email == Email);
                }

                if (emailRecipient is null)
                {
                    return;
                }

                Update(emailRecipient);
            }

            private void Update(EmailRecipient emailRecipient)
            {
                if (Status == EmailStatus.Delivered)
                {
                    emailRecipient.DeliverDateTime = DateTime;
                }
                else if (Status == EmailStatus.Opened)
                {
                    emailRecipient.OpenDateTime = DateTime;
                }

                if (emailRecipient.Status < Status)
                {
                    emailRecipient.Status = Status;
                }
            }
        }
    }
}