using WhyNotEarth.Meredith.Tests.Data;
using WhyNotEarth.Meredith.Public;
using Xunit;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;
using WhyNotEarth.Meredith.Exceptions;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.Tests.Public.CompanyServiceTests
{
    public class CompanyServiceTests : DatabaseContextTest
    {
        private CompanyService CompanyService { get; }

        public CompanyServiceTests()
        {
            CompanyService = ServiceProvider.GetRequiredService<CompanyService>();
        }

        [Fact]
        public async Task ThrowsNameRequired()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await CompanyService.CreateCompanyAsync("", ""));
            Assert.Equal("Name cannot be empty", exception.Message);
        }

        [Fact]
        public async Task ThrowsNameCannotBeNull()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await CompanyService.CreateCompanyAsync(null, null));
            Assert.Equal("name", exception.ParamName);
        }

        [Fact]
        public async Task ThrowsCompanyNameExists()
        {
            var company = CreateAndSaveCompany();
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await CompanyService.CreateCompanyAsync("test", ""));
            Assert.Equal("Company with that name already exists", exception.Message);
        }

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
    }
}
