using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.Public
{
    public class CompanyService
    {
        protected MeredithDbContext Context { get; }

        public CompanyService(
            MeredithDbContext meredithDbContext)
        {
            Context = meredithDbContext;
        }

        public async Task<Company> CreateCompanyAsync(string name, string slug)
        {
            if (name == null)
                throw new ArgumentNullException("name", "Name cannot be null");

            if (name.Trim() == string.Empty)
                throw new ArgumentException("Name cannot be empty");

            var companyExists = await Context.Companies.AnyAsync(a => a.Name.Equals(name));

            if (companyExists)
                throw new ArgumentException("Company with that name already exists");

            var company = new Company()
            {
                Name = name,
                Slug = slug
            };

            Context.Companies.Add(company);
            await Context.SaveChangesAsync();
            return company;
        }
    }
}
