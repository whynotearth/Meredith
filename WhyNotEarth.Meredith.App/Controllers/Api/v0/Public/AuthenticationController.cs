using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Authentication;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Identity.Models;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Controllers.Api.v0.Public
{
    [ApiVersion("0")]
    [Route("/api/v0/authentication")]
    [ProducesErrorResponseType(typeof(void))]
    public class AuthenticationController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager,
            IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        [Returns401]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            User? user = null;
            var userName = model.UserName;

            if (model.UserName is null)
            {
                user = await _userManager.FindByEmailAsync(model.Email);

                if (user is null)
                {
                    return Unauthorized(new { error = "You have entered an invalid username or password" });
                }

                userName = user.UserName;
            }

            var result = await _signInManager.PasswordSignInAsync(userName, model.Password, true, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { error = "You have entered an invalid username or password" });
            }

            if (user is null)
            {
                user = await _userManager.FindByNameAsync(userName);
            }

            var jwtToken = await _userService.GenerateJwtTokenAsync(user);

            return Ok(jwtToken);
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

                var currentUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

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
                    EmailConfirmed = true
                };

                _userService.Map(user, info);

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    return Error("Error creating user", createResult.Errors);
                }
            }
            else if (!user.EmailConfirmed)
            {
                user.EmailConfirmed = true;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return Error("Error updating user", updateResult.Errors);
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
        [HttpGet("ping")]
        public async Task<ActionResult<List<PingResult>>> Ping()
        {
            var user = await _userManager.GetUserAsync(User);
            var logins = await _userManager.GetLoginsAsync(user);

            return Ok(new PingResult(user, logins.Select(item => item.LoginProvider).ToList()));
        }

        [HttpPost("forgotpassword")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            await _userService.SendForgotPasswordAsync(model);

            return Ok();
        }

        [HttpPost("forgotpasswordreset")]
        public async Task<IActionResult> ForgotPasswordReset(ForgotPasswordResetModel model)
        {
            var identityResult = await _userService.ForgotPasswordResetAsync(model);

            return OkIdentityResult(identityResult);
        }

        [Authorize]
        [Returns400]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
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

            return OkIdentityResult(identityResult);
        }

        [Authorize]
        [Returns204]
        [Returns404]
        [HttpPost("confirmphonetoken")]
        public async Task<NoContentResult> SendConfirmPhoneNumberToken(ConfirmPhoneNumberTokenModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            await _userService.SendConfirmPhoneNumberTokenAsync(user, model);

            return NoContent();
        }

        [Authorize]
        [Returns204]
        [Returns400]
        [HttpPost("confirmphone")]
        public async Task<IActionResult> ConfirmPhoneNumber(ConfirmPhoneNumberModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var identityResult = await _userService.ConfirmPhoneNumberAsync(user, model);

            return NoContentIdentityResult(identityResult);
        }

        [Authorize]
        [Returns204]
        [Returns404]
        [HttpPost("confirmemailtoken")]
        public async Task<NoContentResult> SendConfirmEmailToken(ConfirmEmailTokenModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            await _userService.SendConfirmEmailTokenAsync(user, model);

            return NoContent();
        }

        [Authorize]
        [Returns204]
        [Returns400]
        [HttpPost("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var identityResult = await _userService.ConfirmEmailAsync(user, model);

            return NoContentIdentityResult(identityResult);
        }

        private async Task<ActionResult<string>> SignIn(User user)
        {
            await _signInManager.SignInAsync(user, true);

            return Ok(await _userService.GenerateJwtTokenAsync(user));
        }

        private async Task<RedirectResult> RedirectWithJwtAsync(User user, string? returnUrl)
        {
            var jwtToken = await _userService.GenerateJwtTokenAsync(user);
            var finalReturnUrl = UrlHelper.AddQueryString(returnUrl, new Dictionary<string, string>
            {
                {"token", jwtToken}
            });

            return Redirect(finalReturnUrl);
        }

        private IActionResult OkIdentityResult(IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
            {
                return Ok();
            }

            return BadRequest(identityResult.Errors);
        }

        private IActionResult NoContentIdentityResult(IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
            {
                return NoContent();
            }

            return BadRequest(identityResult.Errors);
        }

        private UnauthorizedObjectResult Error(string message, IEnumerable<IdentityError> identityErrors)
        {
            var errors = string.Join(",", identityErrors.Select(e => e.Description).ToList());

            return Unauthorized(new { error = $"{message}: {errors}" });
        }
    }
}