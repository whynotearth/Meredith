using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Page;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Pages;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("api/v0/pages")]
    [ProducesErrorResponseType(typeof(void))]
    public class PageController : ControllerBase
    {
        private readonly PageService _pageService;

        public PageController(PageService pageService)
        {
            _pageService = pageService;
        }

        [Returns404]
        [HttpGet("slug/{companySlug}/{pageSlug}")]
        public async Task<PageResult> Get(string companySlug, string pageSlug)
        {
            var page = await _pageService.GetPageAsync(companySlug, pageSlug);

            if (page is null)
            {
                throw new RecordNotFoundException();
            }

            return new PageResult(page, GetCulture());
        }

        [Returns404]
        [HttpGet("slug/{companySlug}/{tenantSlug}/{pageSlug}")]
        public async Task<ActionResult<PageResult>> GetTenant(string companySlug, string tenantSlug, string pageSlug)
        {
            var page = await _pageService.GetPageAsync(companySlug, tenantSlug, pageSlug);

            if (page is null)
            {
                throw new RecordNotFoundException();
            }

            return new PageResult(page, GetCulture());
        }

        [Returns404]
        [HttpGet("slug/{companySlug}")]
        public async Task<List<PageResult>> GetPages(string companySlug)
        {
            var pages = await _pageService.GetPagesAsync(companySlug);

            if (pages.Count == 0)
            {
                throw new RecordNotFoundException();
            }

            var culture = GetCulture();
            var result = pages.Select(item => new PageResult(item, culture)).ToList();

            return result;
        }

        [Returns404]
        [HttpGet("slug/{companySlug}/categories/by-name/{categoryName}")]
        public async Task<List<PageResult>> ByCompanyByCategoryName(string companySlug, string categoryName)
        {
            var pages = await _pageService.GetPagesAsync(companySlug, categoryName);

            if (pages.Count == 0)
            {
                throw new RecordNotFoundException();
            }

            var culture = GetCulture();
            var result = pages.Select(item => new PageResult(item, culture)).ToList();

            return result;
        }

        [Returns404]
        [HttpGet("slug/{companySlug}/{pageSlug}/landingpage")]
        public async Task<string> GetLandingPageData(string companySlug, string pageSlug)
        {
            var page = await _pageService.GetLandingPageAsync(companySlug, pageSlug);

            if (page is null)
            {
                throw new RecordNotFoundException($"Page {pageSlug} in company {companySlug} not found");
            }

            return page.LandingPageData ?? string.Empty;
        }

        private string GetCulture()
        {
            return Request.HttpContext.Features.Get<IRequestCultureFeature>()
                .RequestCulture.Culture.Name;
        }
    }
}