namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using WhyNotEarth.Meredith.Cloudinary;

    [ApiVersion("0")]
    [Route("/api/v0/planetcollage")]
    [EnableCors]
    public class PlanetCollageController : Controller
    {
        protected CloudinaryOptions CloudinaryOptions { get; }

        public PlanetCollageController(IOptions<CloudinaryOptions> cloudinaryOptions)
        {
            CloudinaryOptions = cloudinaryOptions.Value;
        }

        protected List<SearchResource> GetResources(string tag, int totalCount)
        {
            var cloudinary = new Cloudinary(new Account(CloudinaryOptions.CloudName, CloudinaryOptions.ApiKey, CloudinaryOptions.ApiSecret));
            var resources = new List<SearchResource>();
            string nextCursor = null;
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
            }
            while (totalCount > 0);
            return resources;
        }

        [HttpGet]
        [Route("by-tag/{tag}")]
        public IActionResult Get(string tag)
        {
            var resources = GetResources(tag, 800);
            var imageSize = 75;
            return Ok(resources.Select(r => new
            {
                Id = r.PublicId,
                Url = new Url(CloudinaryOptions.CloudName)
                    .Transform(new Transformation()
                        .Width(imageSize)
                        .Height(imageSize)
                        .Crop("fill")
                        .Effect("grayscale"))
                    .Secure(true)
                    .BuildUrl(r.PublicId)
            }).ToList());
        }

        [HttpPost]
        [Route("full")]
        public IActionResult FullResolution([FromBody] FullResolutionModel model)
        {
            return Ok(new
            {
                model.Id,
                Url = new Url(CloudinaryOptions.CloudName)
                    .Secure(true)
                    .BuildUrl(model.Id)
            });
        }

        public class FullResolutionModel
        {
            public string Id { get; set; }
        }
    }
}