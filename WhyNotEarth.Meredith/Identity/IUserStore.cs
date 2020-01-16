namespace WhyNotEarth.Meredith.Identity
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using WhyNotEarth.Meredith.Data.Entity.Models;

    public interface IUserStore : IUserStore<User>
    {
        Task<User> FindByDomainAsync(string domain, string userName);
        Task<User> FindByLoginDomainAsync(string domain, string loginProvider, string providerKey);
    }
}