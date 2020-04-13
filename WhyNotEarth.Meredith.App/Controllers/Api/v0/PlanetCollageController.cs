using System;
using System.Collections.Generic;
using System.Linq;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.Cloudinary;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    [ApiVersion("0")]
    [Route("/api/v0/planetcollage")]
    [ProducesErrorResponseType(typeof(void))]
    public class PlanetCollageController : ControllerBase
    {
        private readonly CloudinaryOptions _cloudinaryOptions;

        public PlanetCollageController(IOptions<CloudinaryOptions> cloudinaryOptions)
        {
            _cloudinaryOptions = cloudinaryOptions.Value;
        }

        private List<SearchResource> GetResources(string tag, int totalCount)
        {
            var cloudinary = new CloudinaryDotNet.Cloudinary(new Account(_cloudinaryOptions.CloudName,
                _cloudinaryOptions.ApiKey, _cloudinaryOptions.ApiSecret));
            var resources = new List<SearchResource>();
            string? nextCursor = null;
            do
            {
                var requestSize = Math.Min(totalCount, 500);
                var results = cloudinary.Search()
                    .Expression($"tags={tag}")
                    .MaxResults(requestSize)
                    .NextCursor(nextCursor)
                    .Execute();
                resources.AddRange(results.Resources);
                totalCount -= 500;
                nextCursor = results.NextCursor;
                if (results.Resources.Count == 0 || results.Resources.Count != requestSize)
                {
                    break;
                }
            } while (totalCount > 0);

            return resources;
        }

        [HttpGet("by-tag/{tag}")]
        public IActionResult Get(string tag)
        {
            var resources = GetResources(tag, 800);
            const int imageSize = 75;
            return Ok(resources.Select(r => new
            {
                Id = r.PublicId,
                Url = new Url(_cloudinaryOptions.CloudName)
                    .Transform(new Transformation()
                        .Width(imageSize)
                        .Height(imageSize)
                        .Crop("fill")
                        .Effect("grayscale"))
                    .Secure()
                    .BuildUrl(r.PublicId)
            }).ToList());
        }

        [HttpPost("full")]
        public IActionResult FullResolution(FullResolutionModel model)
        {
            return Ok(new
            {
                model.Id,
                Url = new Url(_cloudinaryOptions.CloudName)
                    .Secure()
                    .BuildUrl(model.Id)
            });
        }

        public class FullResolutionModel
        {
            public string Id { get; set; }
        }
    }
}