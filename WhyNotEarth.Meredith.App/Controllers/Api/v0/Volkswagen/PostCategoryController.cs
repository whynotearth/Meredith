using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen.PostCategory;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/categories")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class PostCategoryController : ControllerBase
    {
        private readonly MeredithDbContext _dbContext;

        public PostCategoryController(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<PostCategoryResult>>> List()
        {
            var categories = await _dbContext.Categories.Where(item => item is PostCategory).ToListAsync();

            return Ok(categories.Select(item => new PostCategoryResult(item)));
        }
    }
}