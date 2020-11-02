using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/categories")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class ArticleCategoryController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        public ArticleCategoryController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public async Task<List<ArticleCategoryResult>> List()
        {
            var categories = await _dbContext.Categories.Include(item => item.Image).OfType<ArticleCategory>()
                .ToListAsync();

            return categories.Select(item => new ArticleCategoryResult(item)).ToList();
        }
    }
}