namespace WhyNotEarth.Meredith.Identity
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using WhyNotEarth.Meredith.Data.Entity.Models;
    using WhyNotEarth.Meredith.Notifications.Email;

    public class UserManager : UserManager<User>
    {
        protected IEmailService EmailService { get; }

        public UserManager(
            IEmailService emailService,
            IUserStore<User> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            EmailService = emailService;
        }

        public async Task ForgotPassword(string userName)
        {
            var user = await Store.FindByNameAsync(userName, CancellationToken);
            if (user == null)
            {
                return;
            }

            var token = await GeneratePasswordResetTokenAsync(user);
            await EmailService.SendPasswordResetEmail(user, token);
        }

        public async Task<bool> ResetPassword(string token, Guid userId, string newPassword)
        {
            var user = await Store.FindByIdAsync(userId.ToString(), CancellationToken);
            if (user == null)
            {
                return false;
            }

            var result = await ResetPasswordAsync(user, token, newPassword);
            return result.Succeeded;
        }
    }
}