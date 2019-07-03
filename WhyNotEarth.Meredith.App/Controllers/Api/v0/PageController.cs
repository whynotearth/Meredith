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

        private IQueryable<Page> PageIncludes() => MeredithDbContext.Pages
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
            .Include(p => p.Images);

        public PageController(MeredithDbContext meredithDbContext)
        {
            MeredithDbContext = meredithDbContext;
        }

        [HttpGet]
        [Route("slug/{companySlug}/{pageSlug}")]
        public async Task<IActionResult> Get(string companySlug, string pageSlug)
        {
            var page = await PageIncludes()
                .FirstOrDefaultAsync(p =>
                    p.Company.Slug.ToLower() == companySlug.ToLower()
                    && p.Slug.ToLower() == pageSlug.ToLower());
            if (page == null)
            {
                return NotFound();
            }

            return Ok(new[] { page }.AsQueryable().Select(PageToReturn).FirstOrDefault());
        }

        [HttpGet]
        [Route("slug/{companySlug}")]
        public async Task<IActionResult> GetPages(string companySlug)
        {
            var pages = await PageIncludes()
                .Where(p =>
                    p.Company.Slug.ToLower() == companySlug.ToLower())
                .ToListAsync();
            if (pages.Count == 0)
            {
                return NotFound();
            }

            return Ok(pages.AsQueryable().Select(PageToReturn).ToList());
        }

        [HttpGet]
        [Route("slug/{companySlug}/categories/by-name/{categoryName}")]
        public async Task<IActionResult> ByCompanyByCategoryName(string companySlug, string categoryName)
        {
            var pages = await PageIncludes()
                .Where(p => p.Company.Slug == companySlug
                    && p.Category.Name == categoryName)
                .ToListAsync();
            return Ok(pages.AsQueryable().Select(PageToReturn).ToList());
        }

        private static string GetCardType(Card.CardTypes cardType)
        {
            switch (cardType)
            {
                case Card.CardTypes.Card:
                    return "story-card";
                default:
                    throw new Exception($"Card type {cardType} not mapped.");
            }
        }

        private Func<Page, dynamic> PageToReturn = (page) => new
        {
            Id = page.Id,
            Brand = page.Company.Slug,
            Name = page.Name,
            Title = page.Title,
            page.Description,
            H2 = page.Header,
            Slug = page.Slug,
            BackgroundImage = page.BackgroundImage,
            FeaturedImage = page.FeaturedImage,
            Images = page.Images
                    .OrderBy(i => i.Order)
                    .Select(i => new
                    {
                        i.Order,
                        i.Url
                    }).ToArray(),
            CtaText = page.CallToAction,
            CtaLink = page.CallToActionLink,
            Stories = page.Cards
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
            Modules = new
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
                        Id = (int)b.BedType,
                        Type = b.BedType.ToString()
                    }).ToList(),
                    Rules = page.Hotel?.Rules.Select(r => r.Text).ToList(),
                    Spaces = page.Hotel?.Spaces.Select(s => s.Name).ToList()
                }
            }
        };
    }
}