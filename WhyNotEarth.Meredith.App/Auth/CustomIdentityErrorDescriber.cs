using Microsoft.AspNetCore.Identity;

namespace WhyNotEarth.Meredith.App.Auth
{
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError InvalidToken()
        {
            return new IdentityError
            {
                Code = nameof(InvalidToken),
                Description = "The link is invalid."
            };
        }
    }
}