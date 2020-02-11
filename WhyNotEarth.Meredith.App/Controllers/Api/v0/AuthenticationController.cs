namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Net;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using WhyNotEarth.Meredith.App.Configuration;
    using WhyNotEarth.Meredith.App.Models.Api.v0.Authentication;
    using WhyNotEarth.Meredith.Data.Entity.Models;

    [ApiVersion("0")]
    [Route("/api/v0/authentication")]
    [EnableCors]
    public class AuthenticationController : Controller
    {
        private SignInManager<User> SignInManager { get; }
        private UserManager<User> UserManager { get; }
        private JwtOptions JwtOptions { get; }

        public AuthenticationController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IOptions<JwtOptions> jwtOptions
            )
        {
            UserManager = userManager;
            SignInManager = signInManager;
            JwtOptions = jwtOptions.Value;
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                var appUser = await UserManager.Users.SingleOrDefaultAsync(r => r.Email == model.Email);
                return Ok(GenerateJwtToken(model.Email, appUser));
            }

            return BadRequest();
        }

        [Route("logout")]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return Ok();
        }

        [Route("provider/login")]
        [HttpGet]
        public IActionResult ProviderLogin(string provider, string returnUrl = null)
        {
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, $"/api/v0/authentication/provider/callback?returnUrl={returnUrl}");
            return new ChallengeResult(provider, properties);
        }

        [Route("provider/callback")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ProviderCallback(string remoteError = null, string returnUrl = null)
        {
            if (remoteError != null)
            {
                return Unauthorized(new { error = $"Error from external provider: {remoteError}" });
            }

            var info = await SignInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Login failed, typically because they cancelled.
                return Redirect(returnUrl);
            }

            if (!info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                return Unauthorized(new { error = "Provider did not return an e-mail address" });
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await UserManager.FindByEmailAsync(email);
            var result = await SignInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                await SignInManager.UpdateExternalAuthenticationTokensAsync(info);
                return Redirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return Unauthorized(new { error = "User is locked out" });
            }
            else
            {
                if (user != null)
                {
                    var addLoginResult = await UserManager.AddLoginAsync(user, info);
                    if (addLoginResult.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, true);
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        var errors = string.Join(",", addLoginResult.Errors.Select(e => e.Description).ToList());
                        return Unauthorized(new { error = $"Error adding login to user: {errors}" });
                    }
                }
                else
                {
                    user = new User
                    {
                        UserName = email,
                        Email = email
                    };
                    var createResult = await UserManager.CreateAsync(user);
                    if (createResult.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, true);
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        var errors = string.Join(",", createResult.Errors.Select(e => e.Description).ToList());
                        return Unauthorized(new { error = $"Error creating user: {errors}" });
                    }
                }
            }
        }


        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };
            var result = await UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, true);
                return Ok(GenerateJwtToken(model.Email, user));
            }

            return BadRequest(result.Errors);
        }

        [Route("ping")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Ping()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var user = await UserManager.GetUserAsync(User);
            return Ok(new
            {
                user.Id,
                user.UserName,
                User.Identity.IsAuthenticated
            });
        }

        private string GenerateJwtToken(string email, User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(JwtOptions.ExpireDays);

            var token = new JwtSecurityToken(
                JwtOptions.Issuer,
                JwtOptions.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}