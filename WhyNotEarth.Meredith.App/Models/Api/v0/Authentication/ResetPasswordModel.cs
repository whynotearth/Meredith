namespace WhyNotEarth.Meredith.App.Models.Api.v0.Authentication
{
    using System;

    public class ResetPasswordModel
    {
        public Guid UserId { get; set; }

        public string Password { get; set; }
    }
}
