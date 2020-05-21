using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.Jobs.Volkswagen
{
    public class JumpStartJob
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly MeredithDbContext _dbContext;
        private readonly JumpStartPlanService _jumpStartPlanService;
        private readonly JumpStartService _jumpStartService;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly SettingsService _settingsService;

        public static string Id { get; } = "JumpStartService_SendAsync";

        // Every 15 minutes
        public static string CronExpression { get; } = "*/15 * * * *";

        public JumpStartJob(MeredithDbContext dbContext, IBackgroundJobClient backgroundJobClient,
            JumpStartPlanService jumpStartPlanService, JumpStartService jumpStartService,
            SettingsService settingsService, IRecurringJobManager recurringJobManager)
        {
            _dbContext = dbContext;
            _backgroundJobClient = backgroundJobClient;
            _jumpStartPlanService = jumpStartPlanService;
            _jumpStartService = jumpStartService;
            _settingsService = settingsService;
            _recurringJobManager = recurringJobManager;
        }

        public async Task SendAsync()
        {
            var jumpStarts = new List<JumpStart>();
            var settings = await _settingsService.GetValueAsync<VolkswagenSettings>(VolkswagenCompany.Slug);

            var dailyPlans = await _jumpStartPlanService.GetAsync();
            dailyPlans = dailyPlans.Where(item => item.DateTime < DateTime.UtcNow.AddMinutes(15)).ToList();

            foreach (var dailyPlan in dailyPlans)
            {
                var jumpStart = dailyPlan.JumpStart;

                if (jumpStart is null)
                {
                    if (!settings.EnableAutoSend)
                    {
                        // Do not send these articles because admin didn't schedule it and the auto send is off
                        continue;
                    }

                    jumpStart = await CreateJumpStart(dailyPlan.DateTime, dailyPlan.DistributionGroups,
                        dailyPlan.Articles);
                }

                jumpStart.Status = JumpStartStatus.Sending;
                jumpStarts.Add(jumpStart);
            }

            _dbContext.UpdateRange(jumpStarts);
            await _dbContext.SaveChangesAsync();

            foreach (var jumpStart in jumpStarts)
            {
                _backgroundJobClient.Enqueue<JumpStartPdfJob>(service =>
                    service.CreatePdfAsync(jumpStart.Id));
            }
        }

        private Task<JumpStart> CreateJumpStart(DateTime dateTime, List<string> distributionGroups,
            List<Article> articles)
        {
            return _jumpStartService.CreateOrEditAsync(null, dateTime, distributionGroups, articles);
        }
    }
}