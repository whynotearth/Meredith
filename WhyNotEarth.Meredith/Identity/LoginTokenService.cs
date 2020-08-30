using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    internal class LoginTokenService : ILoginTokenService
    {
        private readonly IDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public LoginTokenService(IDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            var token = CreateToken();

            var loginToken = await _dbContext.LoginTokens.FirstOrDefaultAsync(item => item.UserId == user.Id);

            if (loginToken is null)
            {
                _dbContext.LoginTokens.Add(new LoginToken
                {
                    UserId = user.Id,
                    Token = token,
                    ExpiresAt = DateTime.UtcNow.AddDays(7)
                });
            }
            else
            {
                loginToken.Token = token;
                _dbContext.LoginTokens.Update(loginToken);
            }

            await _dbContext.SaveChangesAsync();
            return token;
        }

        public async Task<User?> ValidateTokenAsync(string token)
        {
            var loginToken =
                await _dbContext.LoginTokens.FirstOrDefaultAsync(item =>
                    item.Token == token && DateTime.UtcNow <= item.ExpiresAt);

            return loginToken?.User;
        }

        private string CreateToken()
        {
            return _userManager.GenerateNewAuthenticatorKey();
        }
    }
}