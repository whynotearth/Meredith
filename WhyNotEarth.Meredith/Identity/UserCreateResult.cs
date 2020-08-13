using Microsoft.AspNetCore.Identity;
using WhyNotEarth.Meredith.Public;

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