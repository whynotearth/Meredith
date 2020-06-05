using Microsoft.AspNetCore.Identity;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.Identity
{
    public class UserCreateResult
    {
        public IdentityResult IdentityResult { get; }

        public User? User { get; }

        public UserCreateResult(IdentityResult identityResult, User? user)
        {
            IdentityResult = identityResult;

            if (identityResult.Succeeded)
            {
                User = user;
            }
        }
    }
}