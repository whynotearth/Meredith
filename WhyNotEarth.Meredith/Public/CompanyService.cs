using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Exceptions;

namespace WhyNotEarth.Meredith.Public
{
    public class CompanyService
    {
        private readonly IDbContext _dbContext;

        public CompanyService(IDbContext dbContext)
        {
            _dbContext = dbContext;
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

            var companyExists = await _dbContext.Companies.AnyAsync(a => a.Name.Equals(name));

            if (companyExists)
            {
                throw new InvalidActionException("Company with that name already exists");
            }

            var company = new Company
            {
                Name = name,
                Slug = slug
            };

            _dbContext.Companies.Add(company);
            await _dbContext.SaveChangesAsync();

            return company;
        }

        public async Task<Company> GetAsync(string companySlug)
        {
            var company = await _dbContext.Companies.FirstOrDefaultAsync(item => item.Slug == companySlug.ToLower());

            if (company is null)
            {
                throw new RecordNotFoundException($"Company {companySlug} not found");
            }

            return company;
        }
    }
}