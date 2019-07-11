namespace WhyNotEarth.Meredith.App.Controllers.Api.v0
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using WhyNotEarth.Meredith.App.Models.Api.v0.Authentication;
    using WhyNotEarth.Meredith.Data.Entity.Models;

    [ApiVersion("0")]
    [Route("/api/v0/authentication")]
    [EnableCors]
    public class AuthenticationController : Controller
    {
        private SignInManager<User> SignInManager { get; }
        private UserManager<User> UserManager { get; }
        private IConfiguration Configuration { get; }

        public AuthenticationController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration configuration
            )
        {
            UserManager = userManager;
            SignInManager = signInManager;
            Configuration = configuration;
        }

        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                var appUser = await UserManager.Users.SingleOrDefaultAsync(r => r.Email == model.Email);
                return Ok(GenerateJwtToken(model.Email, appUser));
            }

            return BadRequest();
        }


        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await SignInManager.SignInAsync(user, false);
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(Configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                Configuration["Jwt:Issuer"],
                Configuration["Jwt:Issuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}