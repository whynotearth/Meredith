using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    // TODO: Fix images
    [Returns401]
    [Returns403]
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/posts")]
    [ProducesErrorResponseType(typeof(void))]
    [Authorize(Policy = Policies.ManageVolkswagen)]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        [Returns200]
        [Returns404]
        [HttpPost("")]
        public async Task<ActionResult<PostResult>> Create(PostModel model)
        {
            var post = await _postService.CreateAsync(model.CategoryId, model.Date, model.Headline,
                model.Description, model.Price, model.EventDate, model.Image);

            return Ok(new PostResult(post));
        }

        [Returns200]
        [HttpGet("")]
        public async Task<ActionResult<List<PostResult>>> GetAvailable(DateTime date)
        {
            var availablePosts = await _postService.GetAvailablePosts(date);

            var result = availablePosts.Select(item => new PostResult(item)).ToList();

            return Ok(result);
        }

        [Returns204]
        [Returns404]
        [HttpPut("{postId}")]
        public async Task<NoContentResult> Edit(int postId, PostModel model)
        {
            await _postService.EditAsync(postId, model.CategoryId, model.Date, model.Headline,
                model.Description, model.Price, model.EventDate);

            return NoContent();
        }

        [Returns204]
        [Returns404]
        [HttpDelete("{postId}")]
        public async Task<NoContentResult> Delete(int postId)
        {
            await _postService.DeleteAsync(postId);

            return NoContent();
        }
    }
}