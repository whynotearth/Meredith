namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using WhyNotEarth.Meredith.Data.Entity;
    using WhyNotEarth.Meredith.Data.Entity.Models;

    [ApiVersion("0")]
    [Route("/api/v0/pages")]
    [EnableCors]
    public class PageController : Controller
    {
        private MeredithDbContext MeredithDbContext { get; }

        public PageController(MeredithDbContext meredithDbContext)
        {
            MeredithDbContext = meredithDbContext;
        }

        [HttpGet]
        [Route("slug/{companySlug}/{pageSlug}")]
        public async Task<IActionResult> Get(string companySlug, string pageSlug)
        {
            var page = await MeredithDbContext.Pages
                .Include(p => p.Company)
                .Include(p => p.Cards)
                .Include(p => p.Hotel)
                .ThenInclude(p => p.Amenities)
                .Include(p => p.Hotel)
                .ThenInclude(p => p.Beds)
                .Include(p => p.Hotel)
                .ThenInclude(p => p.Rules)
                .Include(p => p.Hotel)
                .ThenInclude(p => p.Spaces)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p =>
                    p.Company.Slug.ToLower() == companySlug.ToLower()
                    && p.Slug.ToLower() == pageSlug.ToLower());

            if (page == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = page.Id,
                brand = page.Company.Slug,
                name = page.Name,
                title = page.Title,
                h2 = page.Header,
                slug = page.Slug,
                backgroundImage = page.BackgroundImage,
                featuredImage = page.FeaturedImage,
                images = page.Images
                    .OrderBy(i => i.Order)
                    .Select(i => new
                    {
                        i.Order,
                        i.Url
                    }).ToArray(),
                ctaText = page.CallToAction,
                ctaLink = page.CallToActionLink,
                stories = page.Cards
                    .OrderBy(c => c.Order)
                    .Select(c => new
                    {
                        content = c.Text,
                        ctaText = c.CallToAction,
                        ctaLink = c.CallToActionUrl,
                        image = c.BackgroundUrl,
                        blur = "2px",
                        type = GetCardType(c.CardType)
                    }),
                Custom = page.Custom == null ? null : JsonConvert.DeserializeObject<dynamic>(page.Custom),
                modules = new
                {
                    Hotel = new
                    {
                        page.Hotel?.Capacity,
                        page.Hotel?.GettingAround,
                        page.Hotel?.Location,
                        Amenities = page.Hotel?.Amenities.Select(a => a.Text).ToList(),
                        Beds = page.Hotel?.Beds.Select(b => new
                        {
                            b.Count,
                            Type = b.BedType.ToString()
                        }).ToList(),
                        Rules = page.Hotel?.Rules.Select(r => r.Text).ToList(),
                        Spaces = page.Hotel?.Spaces.Select(s => s.Name).ToList()
                    }
                }

            });
        }

        private string GetCardType(Card.CardTypes cardType)
        {
            switch (cardType)
            {
                case Card.CardTypes.Card:
                    return "story-card";
                default:
                    throw new Exception($"Card type {cardType} not mapped.");
            }
        }
    }
}