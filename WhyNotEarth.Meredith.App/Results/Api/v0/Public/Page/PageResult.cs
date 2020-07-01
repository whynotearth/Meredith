using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WhyNotEarth.Meredith.App.Results.Api.v0.Hotel.Page;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Page
{
    public class PageResult
    {
        public int Id { get; }

        public string Brand { get; }

        public string? Tenant { get; }

        public List<CategoryResult> Categories { get; } = new List<CategoryResult>();

        public string? Name { get; }

        public string? Title { get; }

        public string? Description { get; }

        public string? H2 { get; }

        public string? BackgroundImage { get; }

        public List<ImageResult> Images { get; } = new List<ImageResult>();

        public string? CtaText { get; }

        public string? CtaLink { get; }

        public List<StoryResult> Stories { get; } = new List<StoryResult>();

        public object? Custom { get; }

        public Dictionary<string, object> Modules { get; } = new Dictionary<string, object>();

        public string? FeaturedImage { get; }

        public string? Slug { get; }

        public PageResult(Data.Entity.Models.Page page, string culture)
        {
            Id = page.Id;
            Brand = page.Company.Slug;
            Tenant = page.Tenant?.Slug;
            Name = page.Translations.FirstOrDefault(t => t.Language.Culture == culture)?.Name;
            Title = page.Translations.FirstOrDefault(t => t.Language.Culture == culture)?.Title;
            Description = page.Translations.FirstOrDefault(t => t.Language.Culture == culture)?.Description;
            H2 = page.Translations.FirstOrDefault(t => t.Language.Culture == culture)?.Header;
            BackgroundImage = page.BackgroundImage;
            CtaText = page.Translations.FirstOrDefault(t => t.Language.Culture == culture)?.CallToAction;
            CtaLink = page.CallToActionLink;
            Custom = page.Custom is null ? null : JsonConvert.DeserializeObject<dynamic>(page.Custom);
            FeaturedImage = page.FeaturedImage;
            Slug = page.Slug;

            var imageResults = page.Images.OrderBy(i => i.Order).Select(i => new ImageResult(i));
            Images.AddRange(imageResults);

            var storyResults = page.Cards.OrderBy(c => c.Order).Select(c => new StoryResult(c.Id, c.Text, c.CallToAction,
                c.CallToActionUrl, c.BackgroundUrl, c.PosterUrl, c.CardType));
            Stories.AddRange(storyResults);

            if (page.Category != null)
            {
                Categories.Add(new CategoryResult(page.Category));
            }

            AddHotelModule(page.Hotel, culture);
        }

        private void AddHotelModule(Data.Entity.Models.Modules.Hotel.Hotel? hotel, string culture)
        {
            if (hotel is null)
            {
                return;
            }

            var hotelModule = new HotelModuleResult
            (
                hotel.Id,
                hotel.Translations.FirstOrDefault(t => t.Language.Culture == culture)?.GettingAround,
                hotel.Translations.FirstOrDefault(t => t.Language.Culture == culture)?.Location
            );

            hotelModule.Amenities.AddRange(hotel.Amenities.SelectMany(a => a.Translations)
                .Where(t => t.Language.Culture == culture).Select(t => t.Text));

            hotelModule.Rules.AddRange(hotel.Rules.SelectMany(r => r.Translations)
                .Where(t => t.Language.Culture == culture).Select(t => t.Text));

            hotelModule.Spaces.AddRange(hotel.Spaces.SelectMany(s => s.Translations)
                .Where(t => t.Language.Culture == culture).Select(t => t.Name));

            foreach (var roomType in hotel.RoomTypes)
            {
                var roomTypeResult = new RoomTypeResult(roomType.Id, roomType.Name, roomType.Capacity);

                roomTypeResult.Beds.AddRange(
                    roomType.Beds.Select(b => new BedResult((int) b.BedType, b.BedType.ToString(), b.Count))
                );

                hotelModule.RoomTypes.Add(roomTypeResult);
            }

            Modules.Add("hotel", hotelModule);
        }
    }
}