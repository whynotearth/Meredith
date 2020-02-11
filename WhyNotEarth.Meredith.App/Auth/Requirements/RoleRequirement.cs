namespace WhyNotEarth.Meredith.App.Auth.Requirements
{
    using Microsoft.AspNetCore.Authorization;

    public class RoleRequirement : IAuthorizationRequirement
    {
        public string Role { get; set; }

        public RoleRequirement(string role)
        {
            Role = role;
        }
    }
}
