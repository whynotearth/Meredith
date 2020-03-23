using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Models.Api.v0.Volkswagen.Post;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.App.Results.Api.v0.Volkswagen.Post;
using WhyNotEarth.Meredith.Volkswagen;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Volkswagen
{
    [ApiVersion("0")]
    [Route("api/v0/Volkswagen/posts")]
    public class PostController : ControllerBase
    {
        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        [Route("")]
        [Authorize(Policy = Policies.ManageJumpStart)]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(PostModel postModel)
        {
            await _postService.CreateAsync(postModel.CategoryId, postModel.Date, postModel.Headline,
                postModel.Description, postModel.Images);

            return Ok();
        }

        [HttpGet]
        [Route("")]
        [Authorize(Policy = Policies.ManageJumpStart)]
        [ProducesErrorResponseType(typeof(void))]
        [ProducesResponseType(typeof(List<PostResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailable(DateTime date)
        {
            var availablePosts = await _postService.GetAvailablePosts(date);

            var result = availablePosts.Select(item => new PostResult(item.Id, item.Headline, item.Description, item.Date,
                item.Images)).ToList();

            return Ok(result);
        }
    }
}