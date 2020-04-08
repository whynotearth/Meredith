using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen.PostCategory;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/categories")]
    public class PostCategoryController : ControllerBase
    {
        private readonly MeredithDbContext _dbContext;

        public PostCategoryController(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        [Authorize(Policy = Policies.ManageJumpStart)]
        [ProducesResponseType(typeof(List<PostCategoryResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List()
        {
            var categories = await _dbContext.Categories.Where(item => item is PostCategory).ToListAsync();

            return Ok(categories.Select(item => new PostCategoryResult(item)));
        }
    }
}