using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Persistence;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Auth
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
        {
            var jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services
                .AddIdentity<User, Role>(o =>
                {
                    o.User.RequireUniqueEmail = true;
                    o.Password.RequireNonAlphanumeric = false;
                })
                .AddUserManager<UserManager>()
                .AddRoleManager<RoleManager>()
                .AddEntityFrameworkStores<MeredithDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<CustomIdentityErrorDescriber>();

            services
                .AddAuthentication(o =>
                {
                    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    o.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddJwtBearer(config =>
                {
                    config.RequireHttpsMetadata = false;
                    config.SaveToken = true;
                    config.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddGoogle(options =>
                {
                    var config = configuration.GetSection("Authentication:Google");
                    options.ClientId = config["ClientId"];
                    options.ClientSecret = config["ClientSecret"];
                    options.Events.OnRemoteFailure = HandleOnRemoteFailure;

                    // Profile picture
                    options.Scope.Add("profile");
                    options.Events.OnCreatingTicket = context =>
                    {
                        var picture = context.User.GetProperty("picture").GetString();

                        if (picture is not null)
                        {
                            context.Identity.AddClaim(new Claim("picture", picture));
                        }

                        return Task.CompletedTask;
                    };
                })
                .AddFacebook(options =>
                {
                    var config = configuration.GetSection("Authentication:Facebook");
                    options.ClientId = config["ClientId"];
                    options.ClientSecret = config["ClientSecret"];
                    options.Events.OnRemoteFailure = HandleOnRemoteFailure;

                    // Profile picture
                    options.Fields.Add("picture");
                    options.Events.OnCreatingTicket = context =>
                    {
                        var picture = context.User.GetProperty("picture").GetProperty("data").GetProperty("url")
                            .GetString();

                        if (picture is not null)
                        {
                            context.Identity.AddClaim(new Claim("picture", picture));
                        }

                        return Task.CompletedTask;
                    };
                })
                .AddApple(options =>
                {
                    var config = configuration.GetSection("Authentication:Apple");
                    options.GenerateClientSecret = true;
                    options.ClientId = config["ClientId"];
                    options.KeyId = config["KeyId"];
                    options.TeamId = config["TeamId"];
                    options.UsePrivateKey((keyId) => new PhysicalFileProvider(@"C:\inetpub\").GetFileInfo($"AuthKey_{keyId}.p8"));
                });

            return services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "auth";
                options.Cookie.HttpOnly = false;
                options.Cookie.SameSite = SameSiteMode.None;
                options.LoginPath = null;
            });
        }

        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder app)
        {
            app.UseAuthentication().Use(async (context, next) =>
            {
                // If the default identity failed to authenticate (cookies)
                if (context.User.Identities.All(i => !i.IsAuthenticated))
                {
                    var principal = new ClaimsPrincipal();
                    var jwtAuth = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
                    if (jwtAuth?.Principal != null)
                    {
                        principal.AddIdentities(jwtAuth.Principal.Identities);
                        context.User = principal;
                    }
                }

                await next();
            });

            return app;
        }

        private static Task HandleOnRemoteFailure(RemoteFailureContext context)
        {
            if (context.Properties?.RedirectUri is not null)
            {
                context.Response.Redirect(context.Properties.RedirectUri);
            }

            context.HandleResponse();

            return Task.FromResult(0);
        }
    }
}