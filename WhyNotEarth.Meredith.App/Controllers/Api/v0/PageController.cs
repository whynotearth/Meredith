namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
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
                .FirstOrDefaultAsync(p =>
                    p.Company.Slug == companySlug
                    && p.Slug == pageSlug);

            if (page == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                brand = page.Company.Slug,
                name = page.Name,
                title = page.Title,
                h2 = page.Header,
                slug = page.Slug,
                image = page.BackgroundImage,
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
                    })
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