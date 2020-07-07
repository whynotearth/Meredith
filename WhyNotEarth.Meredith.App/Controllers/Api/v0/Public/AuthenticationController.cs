using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WhyNotEarth.Meredith.App.Configuration;
using WhyNotEarth.Meredith.App.Models.Api.v0.Authentication;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Authentication;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Email;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Models;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("/api/v0/authentication")]
    [ProducesErrorResponseType(typeof(void))]
    public class AuthenticationController : ControllerBase
    {
        private readonly JwtOptions _jwtOptions;
        private readonly SendGridService _sendGridService;
        private readonly IUserService _userService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager,
            IOptions<JwtOptions> jwtOptions, SendGridService sendGridService, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sendGridService = sendGridService;
            _userService = userService;
            _jwtOptions = jwtOptions.Value;
        }

        [Returns200]
        [Returns401]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { error = "You have entered an invalid username or password" });
            }

            var user = await _userManager.FindByNameAsync(model.Email);

            return Ok(await GenerateJwtTokenAsync(model.Email, user));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }

        [HttpGet("provider/login")]
        public IActionResult ProviderLogin(string provider, string? returnUrl = null)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider,
                $"/api/v0/authentication/provider/callback?returnUrl={returnUrl}");
            properties.Parameters.Add("prompt", "select_account");
            return new ChallengeResult(provider, properties);
        }

        [HttpPost("provider/logout")]
        public async Task<IActionResult> ProviderLogout(string provider)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is null)
            {
                return Ok();
            }

            var userLoginInfos = await _userManager.GetLoginsAsync(user);

            var userLoginInfo = userLoginInfos.FirstOrDefault(item => item.LoginProvider == provider);

            if (userLoginInfo is null)
            {
                return Ok();
            }

            await _userManager.RemoveLoginAsync(user, userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);
            await _signInManager.SignOutAsync();

            return Ok();
        }

        [HttpGet("provider/callback")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> ProviderCallback(string? remoteError = null, string? returnUrl = null)
        {
            if (remoteError != null)
            {
                return Unauthorized(new { error = $"Error from external provider: {remoteError}" });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
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

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey,
                false, true);

            if (result.Succeeded)
            {
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                
                var currentUser = await _userManager.FindByEmailAsync(email);

                await UpdateUserAsync(info, currentUser);

                return await RedirectWithJwtAsync(currentUser, returnUrl);
            }

            if (result.IsLockedOut)
            {
                return Unauthorized(new { error = "User is locked out" });
            }

            User user;

            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.GetUserAsync(User);
            }
            else
            {
                user = await _userManager.FindByEmailAsync(email);
            }

            if (user is null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                };

                _userService.Map(user, info);

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return Error("Error creating user", createResult.Errors);
                }
            }

            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {
                return Error("Error adding login to user", addLoginResult.Errors);
            }

            await _signInManager.SignInAsync(user, true);
            return await RedirectWithJwtAsync(user, returnUrl);
        }

        [Returns200]
        [Returns400]
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterModel model)
        {
            var userCreateResult = await _userService.CreateAsync(model);

            if (!userCreateResult.IdentityResult.Succeeded)
            {
                return BadRequest(userCreateResult.IdentityResult.Errors);
            }

            return await SignIn(userCreateResult.User!);
        }

        [Authorize]
        [Returns200]
        [Returns401]
        [HttpGet("ping")]
        public async Task<ActionResult<List<PingResult>>> Ping()
        {
            var user = await _userManager.GetUserAsync(User);
            var logins = await _userManager.GetLoginsAsync(user);

            return Ok(new PingResult(user.Id, user.UserName, User.Identity.IsAuthenticated,
                logins.Select(item => item.LoginProvider).ToList()));
        }

        [HttpPost("forgotpassword")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
            {
                // Don't reveal that the user does not exist
                return Ok();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var uriBuilder = new UriBuilder(model.ReturnUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["email"] = user.Email;
            query["token"] = token;
            uriBuilder.Query = query.ToString();
            var callbackUrl = uriBuilder.ToString();

            await _sendGridService.SendAuthEmail(model.CompanySlug, user.Email, "Reset Password",
                $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return Ok();
        }

        [HttpPost("forgotpasswordreset")]
        public async Task<ActionResult> ForgotPasswordReset(ForgotPasswordResetModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return Ok();
            }

            await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            return Ok();
        }

        [Authorize]
        [Returns400]
        [HttpPost("changepassword")]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            IdentityResult identityResult;
            if (model.OldPassword is null)
            {
                identityResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            }
            else
            {
                identityResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            }

            if (!identityResult.Succeeded)
            {
                return BadRequest(identityResult.Errors);
            }

            return Ok();
        }

        private async Task<ActionResult<string>> SignIn(User user)
        {
            await _signInManager.SignInAsync(user, true);

            return Ok(await GenerateJwtTokenAsync(user.Email, user));
        }

        private async Task<string> GenerateJwtTokenAsync(string email, User user)
        {
            if (!_jwtOptions.IsValid())
            {
                throw new Exception("Missing JWT configurations.");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_jwtOptions.ExpireDays);

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RedirectResult> RedirectWithJwtAsync(User user, string? returnUrl)
        {
            var jwtToken = await GenerateJwtTokenAsync(user.Email, user);
            var finalReturnUrl = AddQueryString(returnUrl, new Dictionary<string, string>
            {
                {"token", jwtToken}
            });

            return Redirect(finalReturnUrl);
        }

        private async Task UpdateUserAsync(ExternalLoginInfo externalLoginInfo, User user)
        {
            user = _userService.Map(user, externalLoginInfo);

            await _userManager.UpdateAsync(user);
        }

        private UnauthorizedObjectResult Error(string message, IEnumerable<IdentityError> identityErrors)
        {
            var errors = string.Join(",", identityErrors.Select(e => e.Description).ToList());

            return Unauthorized(new { error = $"{message}: {errors}" });
        }

        private string? AddQueryString(string? url, Dictionary<string, string> values)
        {
            if (url is null)
            {
                return url;
            }

            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            foreach (var keyValuePair in values)
            {
                query[keyValuePair.Key] = keyValuePair.Value;
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
    }
}