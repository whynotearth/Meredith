using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WhyNotEarth.Meredith.BrowTricks.Services
{
    internal class FormSignatureService : IFormSignatureService
    {
        private readonly IDbContext _dbContext;

        public FormSignatureService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<FormSignature?> GetAsync(Client client, FormTemplateType type)
        {
            return _dbContext.FormSignatures.FirstOrDefaultAsync(
                item => item.ClientId == client.Id && item.Type == type)!;
        }

        public async Task<Dictionary<Client, FormSignature?>> GetAsync(List<Client> clients, FormTemplateType type)
        {
            var ids = clients.Select(item => item.Id);

            var formSignatures = await _dbContext.FormSignatures.Where(
                item => ids.Contains(item.ClientId) && item.Type == type)!.ToListAsync();

            var result = new Dictionary<Client, FormSignature?>();

            foreach (var client in clients)
            {
                result.Add(client, formSignatures.FirstOrDefault(item => item.ClientId == client.Id));
            }

            return result;
        }
    }
}