namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System.Linq;
    using CloudinaryDotNet;
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

        [HttpGet]
        [Route("")]
        public IActionResult Get()
        {
            var cloudinary = new Cloudinary(new Account(CloudinaryOptions.CloudName, CloudinaryOptions.ApiKey, CloudinaryOptions.ApiSecret));
            var result = cloudinary.Search()
                .Expression("folder=Bensley Website/Monday - Disruption/*")
                .Execute();
            var clodinaryUrl = new Url(CloudinaryOptions.CloudName);
            var imageSize = 75;
            return Ok(result.Resources.Select(r => new
            {
                Id = r.PublicId,
                Url = clodinaryUrl
                    .Transform(new Transformation()
                        .Width(imageSize)
                        .Height(imageSize)
                        .Crop("fill"))
                    .BuildUrl(r.PublicId)
            }).ToList());
        }

        [HttpPost]
        [Route("full")]
        public IActionResult FullResolution([FromBody] FullResolutionModel model)
        {
            var clodinaryUrl = new Url(CloudinaryOptions.CloudName);
            return Ok(new
            {
                Id = model.PublicId,
                Url = clodinaryUrl.BuildUrl(model.PublicId)
            });
        }

        public class FullResolutionModel
        {
            public string PublicId { get; set; }
        }
    }
}