using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Public
{
    public class CompanyService
    {
        protected IDbContext Context { get; }

        public CompanyService(IDbContext IDbContext)
        {
            Context = IDbContext;
        }

        public async Task<Company> CreateCompanyAsync(string name, string slug)
        {
            if (name == null)
            {
                throw new InvalidActionException("Name must be provided");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidActionException("Name cannot be empty");
            }

            var companyExists = await Context.Companies.AnyAsync(a => a.Name.Equals(name));

            if (companyExists)
            {
                throw new InvalidActionException("Company with that name already exists");
            }

            var company = new Company
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