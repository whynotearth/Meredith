using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Jobs.Public;

namespace WhyNotEarth.Meredith.Volkswagen.Jobs
{
    public class NewJumpStartJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;

        public static string Id { get; } = "NewJumpStartJob_SendAsync";

        // Every 15 minutes
        public static string CronExpression { get; } = "*/15 * * * *";

        public NewJumpStartJob(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task SendAsync()
        {
            var newJumpStarts = await _dbContext.NewJumpStarts
                .Where(item => item.Status == NewJumpStartStatus.Preview &&
                               item.DateTime < DateTime.UtcNow.AddMinutes(15)).ToListAsync();

            foreach (var newJumpStart in newJumpStarts)
            {
                newJumpStart.Status = NewJumpStartStatus.Sending;
            }

            _dbContext.UpdateRange(newJumpStarts);
            await _dbContext.SaveChangesAsync();

            foreach (var newJumpStart in newJumpStarts)
            {
                _backgroundJobClient.Enqueue<EmailRecipientJob>(job =>
                    job.CreateForNewJumpStart(newJumpStart.Id));
            }
        }
    }
}