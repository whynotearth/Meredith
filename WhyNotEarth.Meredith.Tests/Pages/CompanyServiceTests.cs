namespace WhyNotEarth.Meredith.Tests.Pages
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Company;
    using Meredith.Pages;
    using Meredith.Pages.Data;
    using Microsoft.Extensions.Options;
    using Moq;
    using Xunit;

    public class CompanyServiceTests
    {
        public static IList<object[]> LoadBusinessPageTests = new List<object[]>
        {
            new object[] { "testbusiness", null, true },
            new object[] { "Testbusiness", null, true },
            new object[] { "testbusiness", "test", true },
            new object[] { "testbusiness", "Test", true },
            new object[] { "testbusiness", "a", false },
            new object[] { "a", null, false }
        };
        
        protected CompanyService CompanyService { get; }
        
        public CompanyServiceTests()
        {
            var configMock = new Mock<IOptions<PageDatabaseOptions>>();
            configMock
                .Setup(c => c.Value.Url)
                .Returns("https://storage.googleapis.com/meredith-config/tests2.json");
            var pageDatabase = new PageDatabase(configMock.Object);
            CompanyService = new CompanyService(pageDatabase);
        }
        
        [Theory]
        [MemberData(nameof(LoadBusinessPageTests))]
        public async Task LoadBusinessPage(string businessKey, string pageKey, bool isNotNull)
        {
            var page = await CompanyService.GetCompanyPage(businessKey, pageKey);
            Assert.Equal(isNotNull, page != null);
        }
    }
}