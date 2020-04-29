using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/memo/sendgrid/webhook")]
    [ProducesErrorResponseType(typeof(void))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class SendGridController : ControllerBase
    {
        private readonly MeredithDbContext _dbContext;

        public SendGridController(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("")]
        public async Task<OkResult> Create(List<EventItem> events)
        {
            foreach (var eventList in events.Batch(100))
            {
                foreach (var eventItem in eventList)
                {
                    if (eventItem.MemoId is null)
                    {
                        continue;
                    }

                    var memoRecipient = await _dbContext.MemoRecipients.FirstOrDefaultAsync(item =>
                        item.MemoId == eventItem.MemoId.Value && item.Email == eventItem.Email);

                    if (memoRecipient is null)
                    {
                        throw new Exception($"{nameof(MemoRecipient)} Id:{eventItem.MemoId} Email:{eventItem.Email} not found!");
                    }

                    if (eventItem.Status == MemoStatus.Delivered)
                    {
                        memoRecipient.DeliverDateTime = eventItem.DateTime;
                    }
                    else if (eventItem.Status == MemoStatus.Opened)
                    {
                        memoRecipient.OpenDateTime = eventItem.DateTime;
                    }

                    if (memoRecipient.Status < eventItem.Status)
                    {
                        memoRecipient.Status = eventItem.Status;
                    }
                }

                await _dbContext.SaveChangesAsync();
            }

            return Ok();
        }

        public class EventItem
        {
            // Schema: https://sendgrid.com/docs/for-developers/tracking-events/event/

            [JsonProperty(nameof(MemoRecipient.MemoId))]
            public int? MemoId { get; set; }

            public int Timestamp { get; set; }

            public string Email { get; set; } = null!;

            public string Event { get; set; } = null!;

            public MemoStatus Status
            {
                get
                {
                    return Event switch
                    {
                        "delivered" => MemoStatus.Delivered,
                        "open" => MemoStatus.Opened,
                        "click" => MemoStatus.Clicked,
                        _ => MemoStatus.None
                    };
                }
            }

            public DateTime DateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).UtcDateTime;
        }
    }
}