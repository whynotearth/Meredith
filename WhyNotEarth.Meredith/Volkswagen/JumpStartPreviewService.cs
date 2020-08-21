using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Pdf;

namespace WhyNotEarth.Meredith.Volkswagen
{
    public class JumpStartPreviewService
    {
        private readonly IDbContext _dbContext;
        private readonly JumpStartPlanService _jumpStartPlanService;
        private readonly JumpStartEmailTemplateService _jumpStartEmailTemplateService;
        private readonly IHtmlService _htmlService;

        public JumpStartPreviewService(IDbContext dbContext, JumpStartPlanService jumpStartPlanService, JumpStartEmailTemplateService jumpStartEmailTemplateService, IHtmlService htmlService)
        {
            _dbContext = dbContext;
            _jumpStartPlanService = jumpStartPlanService;
            _jumpStartEmailTemplateService = jumpStartEmailTemplateService;
            _htmlService = htmlService;
        }

        public async Task<byte[]> CreatePreviewAsync(DateTime date, List<int> articleIds)
        {
            if (articleIds.Count > _jumpStartPlanService.MaximumArticlesPerDayCount)
            {
                throw new InvalidActionException(
                    $"Maximum {_jumpStartPlanService.MaximumArticlesPerDayCount} articles are allowed per email");
            }

            var articles = await _dbContext.Articles
                .Include(item => item.Image)
                .Include(item => item.Category)
                .ThenInclude(item => item.Image)
                .Where(item => articleIds.Contains(item.Id)).ToListAsync();

            // Keep the order of the articles
            articles = articles.OrderBy(item => articleIds.IndexOf(item.Id)).ToList();

            var emailTemplate = _jumpStartEmailTemplateService.GetEmailHtml(date, articles, null);

            return await _htmlService.ToPngAsync(emailTemplate);
        }
    }
}