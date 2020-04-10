using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly SendGridService _sendGridService;
        private readonly MeredithDbContext _dbContext;

        public JumpStartService(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient, SendGridService sendGridService)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
            _sendGridService = sendGridService;
        }

        public async Task CreateAsync(DateTime dateTime, List<int> postIds)
        {
            var posts = await _dbContext.Posts.Where(item => postIds.Contains(item.Id))
                .ToListAsync();

            var isValid = posts.All(item => item.Date.Date <= dateTime.Date && item.JumpStartId == null) &&
                          posts.Count == postIds.Count;
            if (!isValid)
            {
                throw new InvalidActionException("Invalid posts");
            }

            var jumpStart = new JumpStart
            {
                DateTime = dateTime,
                Status = JumpStartStatus.ReadyToSend,
                Posts = posts
            };

            foreach (var post in posts)
            {
                post.JumpStart = jumpStart;
                post.Order = postIds.IndexOf(post.Id);
            }

            _dbContext.UpdateRange(posts);

            await _dbContext.SaveChangesAsync();

            _backgroundJobClient.Schedule<JumpStartEmailService>(service =>
                service.SendAsync(jumpStart.Id), DateTime.UtcNow - dateTime);

            _backgroundJobClient.Enqueue<JumpStartPdfService>(service =>
                service.CreatePdfAsync(jumpStart.Id));
        }

        public async Task SendTestAsync(string subject, string date, string to, string description)
        {
            var recipients = await GetRecipients();

            foreach (var batch in SplitList(recipients))
            {
                _backgroundJobClient.Enqueue<JumpStartService>(service =>
                    service.SendTestEmailAsync(subject, date, to, description, batch));
            }
        }

        public async Task SendTestEmailAsync(string subject, string date, string to, string description, List<Recipient> batch)
        {
            var templateData = new Dictionary<string, object>
            {
                {"subject", subject},
                {"date", date},
                {"to", to},
                {"description", description},
            };
            
            
            // Local
            //await _sendGridService.SendEmail("communications@vw.com", batch, "d-90be8f0c2d5a43508c1404b316c19989", templateData);

            // Staging
            await _sendGridService.SendEmail("communications@vw.com", batch, "d-5bf1030c93e04aed850ca9890fcb0b81", templateData);
        }

        private async Task<List<Recipient>> GetRecipients()
        {
            return await _dbContext.Recipients.ToListAsync();
        }

        private IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 10)  
        {        
            for (var i = 0; i < locations.Count; i += nSize) 
            { 
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i)); 
            }  
        } 
    }
}