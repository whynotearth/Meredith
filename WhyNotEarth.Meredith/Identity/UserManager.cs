using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.Identity
{
    public class UserManager : UserManager<User>
    {
        private IDataProtector DataProtector { get; }

        public UserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger,
            IDataProtector dataProtector) : base(store,
            optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services,
            logger)
        {
            DataProtector = dataProtector;
        }

        public new int GetUserId(ClaimsPrincipal principal) => int.Parse(base.GetUserId(principal));

        public int GetUserIdFromToken(string token)
        {
            var unprotectedData = DataProtector.Unprotect(Convert.FromBase64String(token));
            var ms = new MemoryStream(unprotectedData);
            using var reader = new BinaryReader(ms, new UTF8Encoding(false, true), true);
            reader.ReadInt64(); // Read DateTimeOffset that we're going to throw out
            return int.Parse(reader.ReadString());
        }
    }
}