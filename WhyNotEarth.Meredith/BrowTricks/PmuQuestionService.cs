using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Tenant;

namespace WhyNotEarth.Meredith.BrowTricks
{
    internal class PmuQuestionService : IPmuQuestionService
    {
        private readonly MeredithDbContext _dbContext;
        private readonly TenantService _tenantService;

        public PmuQuestionService(MeredithDbContext dbContext, TenantService tenantService)
        {
            _dbContext = dbContext;
            _tenantService = tenantService;
        }

        public async Task CreateAsync(string tenantSlug, PmuQuestionModel model, User user)
        {
            var tenant = await _tenantService.CheckPermissionAsync(user, tenantSlug);

            var pmuQuestion = Map(new PmuQuestion(), tenant, model);

            _dbContext.PmuQuestions.Add(pmuQuestion);
            await _dbContext.SaveChangesAsync();
        }

        public Task<List<PmuQuestion>> ListAsync(string tenantSlug)
        {
            return _dbContext.PmuQuestions
                .Include(item => item.Tenant)
                .Where(item => item.Tenant.Slug == tenantSlug)
                .ToListAsync();
        }

        public async Task EditAsync(string tenantSlug, int questionId, PmuQuestionModel model, User user)
        {
            var (tenant, question) = await GetAsync(user, tenantSlug, questionId);

            var pmuQuestion = Map(question, tenant, model);

            _dbContext.PmuQuestions.Add(pmuQuestion);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string tenantSlug, int questionId, User user)
        {
            var (_, pmuQuestion) = await GetAsync(user, tenantSlug, questionId);

            _dbContext.PmuQuestions.Remove(pmuQuestion);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<(Data.Entity.Models.Tenant, PmuQuestion)> GetAsync(User user, string tenantSlug,
            int questionId)
        {
            var tenant = await _tenantService.CheckPermissionAsync(user, tenantSlug);

            var question = await _dbContext.PmuQuestions
                .FirstOrDefaultAsync(item => item.TenantId == tenant.Id && item.Id == questionId);

            if (question is null)
            {
                throw new RecordNotFoundException($"Question {questionId} not found");
            }

            return (tenant, question);
        }

        private PmuQuestion Map(PmuQuestion pmuQuestion, Data.Entity.Models.Tenant tenant, PmuQuestionModel model)
        {
            pmuQuestion.TenantId = tenant.Id;
            pmuQuestion.Question = model.Question;

            return pmuQuestion;
        }
    }
}