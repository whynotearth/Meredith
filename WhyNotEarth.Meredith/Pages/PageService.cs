using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Pages
{
    public class PageService
    {
        private readonly MeredithDbContext _dbContext;

        public PageService(MeredithDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Page> GetPageAsync(string companySlug, string pageSlug)
        {
            return await Include().FirstOrDefaultAsync(p =>
                p.Company.Slug.ToLower() == companySlug.ToLower()
                && p.Slug.ToLower() == pageSlug.ToLower());
        }

        public async Task<List<Page>> GetPagesAsync(string companySlug)
        {
            return await Include().Where(p => p.Company.Slug.ToLower() == companySlug.ToLower())
                .ToListAsync();
        }

        public async Task<List<Page>> GetPagesAsync(string companySlug, string categoryName)
        {
            return await Include().Where(p => p.Company.Slug == companySlug && p.Category.Name == categoryName)
                .ToListAsync();
        }

        public string GetCardType(Card.CardTypes cardType)
        {
            return cardType switch
            {
                Card.CardTypes.Card => "story-card",
                _ => throw new Exception($"Card type {cardType} not mapped.")
            };
        }

        public async Task<Page> CreateAsync(int companyId, int? categoryId, string slug, string name, string title,
            string header, string featuredImage, string backgroundImage, string callToAction, string callToActionLink,
            string description, string custom)
        {
            var company = _dbContext.Companies.FirstOrDefaultAsync(item => item.Id == companyId);
            if (company is null)
            {
                throw new InvalidActionException("Company does not exist");
            }

            if (categoryId.HasValue)
            {
                var category = _dbContext.Categories.FirstOrDefaultAsync(item => item.Id == categoryId);
                if (category is null)
                {
                    throw new InvalidActionException("Category does not exist");
                }
            }
            
            var page = new Page
            {
                CompanyId = companyId,
                CategoryId = categoryId,
                Slug = slug,
                Name = name,
                Title = title,
                Header = header,
                FeaturedImage = featuredImage,
                BackgroundImage = backgroundImage,
                CallToAction = callToAction,
                CallToActionLink = callToActionLink,
                Description = description,
                Custom = custom
            };
            
            _dbContext.Pages.Add(page);
            await _dbContext.SaveChangesAsync();

            return page;
        }

        public async Task<Page> UpdateAsync(int id, int companyId, int? categoryId, string slug, string name, string title,
            string header, string featuredImage, string backgroundImage, string callToAction, string callToActionLink,
            string description, string custom)
        {
            var page = await _dbContext.Pages.FirstOrDefaultAsync(item => item.Id == id);

            if (page is null)
            {
                throw new RecordNotFoundException("Page does not exist");
            }

            var company = _dbContext.Companies.FirstOrDefaultAsync(item => item.Id == companyId);
            if (company is null)
            {
                throw new RecordNotFoundException("Company does not exist");
            }

            if (categoryId.HasValue)
            {
                var category = _dbContext.Categories.FirstOrDefaultAsync(item => item.Id == categoryId);
                if (category is null)
                {
                    throw new RecordNotFoundException("Category does not exist");
                }
            }

            page.CompanyId = companyId;
            page.CategoryId = categoryId;
            page.Slug = slug;
            page.Name = name;
            page.Title = title;
            page.Header = header;
            page.FeaturedImage = featuredImage;
            page.BackgroundImage = backgroundImage;
            page.CallToAction = callToAction;
            page.CallToActionLink = callToActionLink;
            page.Description = description;
            page.Custom = custom;
            
            _dbContext.Pages.Update(page);
            await _dbContext.SaveChangesAsync();

            return page;
        }

        private IQueryable<Page> Include()
        {
            return _dbContext.Pages
                .Include(p => p.Company)
                .Include(p => p.Cards)
                .Include(p => p.Category)
                .Include(p => p.Hotel)
                .ThenInclude(p => p.Translations)
                .ThenInclude(p => p.Language)
                .Include(p => p.Hotel)
                .ThenInclude(p => p.Amenities)
                .ThenInclude(p => p.Translations)
                .ThenInclude(p => p.Language)
                .Include(p => p.Hotel)
                .ThenInclude(p => p.RoomTypes)
                .Include(p => p.Hotel)
                .ThenInclude(p => p.RoomTypes)
                .ThenInclude(p => p.Beds)
                .Include(p => p.Hotel)
                .ThenInclude(p => p.Rules)
                .ThenInclude(p => p.Translations)
                .ThenInclude(p => p.Language)
                .Include(p => p.Hotel)
                .ThenInclude(p => p.Spaces)
                .ThenInclude(p => p.Translations)
                .ThenInclude(p => p.Language)
                .Include(p => p.Images);
        }

        public async Task<Page> GetLandingPageAsync(string companySlug, string pageSlug)
        {
            var page = await _dbContext.Pages.FirstOrDefaultAsync(p =>
                string.Equals(p.Company.Slug, companySlug, StringComparison.CurrentCultureIgnoreCase)
                && string.Equals(p.Slug, pageSlug, StringComparison.CurrentCultureIgnoreCase));

            return page;
        }
    }
}