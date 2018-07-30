namespace WhyNotEarth.Meredith.Tests.Pages
{
    using System.Linq;
    using System.Threading.Tasks;
    using Meredith.Pages;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class PageDatabaseTests
    {
        public PageDatabaseTests()
        {
            var configMock = new Mock<IOptions<PageDatabaseOptions>>();
            configMock
                .Setup(c => c.Value.Url)
                .Returns("https://storage.googleapis.com/meredith-config/tests2.json");
            PageDatabase = new PageDatabase(configMock.Object);
        }

        protected PageDatabase PageDatabase { get; }

        [Fact]
        public async Task Load()
        {
            await PageDatabase.Load();
            Assert.NotNull(PageDatabase.Businesses);
            Assert.Single(PageDatabase.Businesses);
            var business = PageDatabase.Businesses.First();
            Assert.Equal("TestBusiness", business.Name);
            Assert.Equal("testbusiness", business.Key);
            Assert.NotNull(business.Pages);
            Assert.Equal(2, business.Pages.Count);
            var page = business.Pages.First();
            Assert.Equal("TestPage", page.Name);
            Assert.Null(page.Key);
            Assert.NotNull(page.Cards);
            Assert.Single(page.Cards);
            var card = page.Cards.First();
            Assert.Equal("backgroundUrl", card.BackgroundUrl);
            Assert.Equal("callToAction", card.CallToAction);
            Assert.Equal("callToActionUrl", card.CallToActionUrl);
            Assert.Equal("posterUrl", card.PosterUrl);
        }
    }
}