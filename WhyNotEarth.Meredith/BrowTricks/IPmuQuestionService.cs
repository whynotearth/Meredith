using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.BrowTricks.Models;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public interface IPmuQuestionService
    {
        Task CreateAsync(string tenantSlug, PmuQuestionModel model, User user);

        Task EditAsync(string tenantSlug, int questionId, PmuQuestionModel model, User user);

        Task DeleteAsync(string tenantSlug, int questionId, User user);

        Task<List<PmuQuestion>> ListAsync(string tenantSlug);
    }
}