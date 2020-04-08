using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen.Post;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen.Post;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    // TODO: Fix images
    [ApiVersion("0")]
    [Route("api/v0/volkswagen/posts")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        [HttpPost("")]
        [Authorize(Policy = Policies.ManageJumpStart)]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(PostResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(PostModel model)
        {
            var post = await _postService.CreateAsync(model.CategoryId, model.Date, model.Headline,
                model.Description, model.Price, model.EventDate, model.Images);

            return Ok(new PostResult(post));
        }

        [HttpGet("")]
        [Authorize(Policy = Policies.ManageJumpStart)]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(typeof(List<PostResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailable(DateTime date)
        {
            var availablePosts = await _postService.GetAvailablePosts(date);

            var result = availablePosts.Select(item => new PostResult(item)).ToList();

            return Ok(result);
        }

        [HttpPut("{postId}")]
        [Authorize(Policy = Policies.ManageJumpStart)]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Edit(int postId, PostModel model)
        {
            await _postService.EditAsync(postId, model.CategoryId, model.Date, model.Headline,
                model.Description, model.Price, model.EventDate);

            return NoContent();
        }

        [HttpDelete("{postId}")]
        [Authorize(Policy = Policies.ManageJumpStart)]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int postId)
        {
            await _postService.DeleteAsync(postId);

            return NoContent();
        }
    }
}