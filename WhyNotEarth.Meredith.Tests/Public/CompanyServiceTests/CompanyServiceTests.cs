using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Tests.Data;
using Xunit;

namespace WhyNotEarth.Meredith.Tests.Public.CompanyServiceTests
{
    public class CompanyServiceTests : DatabaseContextTest
    {
        public CompanyServiceTests()
        {
            CompanyService = ServiceProvider.GetRequiredService<CompanyService>();
        }

        private CompanyService CompanyService { get; }

        private async Task<Company> CreateAndSaveCompany()
        {
            var company = new Company
            {
                Name = "test",
                Slug = "test"
            };

            DbContext.Companies.Add(company);
            await DbContext.SaveChangesAsync();
            return company;
        }

        [Fact]
        public async Task ThrowsCompanyNameExists()
        {
            var company = CreateAndSaveCompany();
            var exception =
                await Assert.ThrowsAsync<InvalidActionException>(async () =>
                    await CompanyService.CreateCompanyAsync("test", string.Empty));
            Assert.Equal("Company with that name already exists", exception.Message);
        }

        [Fact]
        public async Task ThrowsNameCannotBeNull()
        {
            var exception = await Assert.ThrowsAsync<InvalidActionException>(async () =>
                await CompanyService.CreateCompanyAsync(string.Empty, string.Empty));
            Assert.Equal("Name cannot be empty", exception.Message);
        }

        [Fact]
        public async Task ThrowsNameRequired()
        {
            var exception =
                await Assert.ThrowsAsync<InvalidActionException>(async () =>
                    await CompanyService.CreateCompanyAsync(string.Empty, string.Empty));
            Assert.Equal("Name cannot be empty", exception.Message);
        }
    }
}