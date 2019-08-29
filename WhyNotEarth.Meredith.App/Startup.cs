namespace WhyNotEarth.Meredith.App
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Data.Entity;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using RollbarDotNet.Configuration;
    using RollbarDotNet.Core;
    using RollbarDotNet.Logger;
    using Stripe.Data;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using WhyNotEarth.Meredith.App.Configuration;
    using WhyNotEarth.Meredith.Data.Entity.Models;
    using WhyNotEarth.Meredith.DependencyInjection;
    using WhyNotEarth.Meredith.Identity;

    public class Startup
    {
        protected IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services
                .AddCors(o => o
                    .AddDefaultPolicy(builder => builder
                        .SetIsOriginAllowed(origin => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()))
                .AddRollbarWeb()
                .AddOptions()
                .Configure<RollbarOptions>(options => Configuration.GetSection("Rollbar").Bind(options))
                .Configure<StripeOptions>(o => Configuration.GetSection("Stripe").Bind(o))
                .Configure<JwtOptions>(o => Configuration.GetSection("Jwt").Bind(o))
                .AddDbContext<MeredithDbContext>(o => o.UseNpgsql(Configuration.GetConnectionString("Default"),
                    options => options.SetPostgresVersion(new Version(9, 6))))
                .AddMeredith()
                .AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User)
                .Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    options.ForwardLimit = null;
                    options.RequireHeaderSymmetry = false;
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                })
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v0", new Info
                    {
                        Title = "Interface API",
                        Version = "v0",
                        Description =
                            "API designed for internal use only, will change and WILL break backwards compability as needed for our GUI"
                    });
                    c.DocInclusionPredicate((docName, apiDesc) =>
                    {
                        apiDesc.TryGetMethodInfo(out var methodInfo);
                        var versions = methodInfo.DeclaringType.GetCustomAttributes(true)
                            .OfType<ApiVersionAttribute>()
                            .SelectMany(attr => attr.Versions);
                        return versions.Any(v => $"v{v.ToString()}" == docName);
                    });
                    c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                    });
                    var security = new Dictionary<string, IEnumerable<string>>
                    {
                        {"Bearer", new string[] { }},
                    };
                    c.AddSecurityRequirement(security);
                });

            var jwtOptions = Configuration.GetSection("Jwt").Get<JwtOptions>();
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services
                .AddAuthentication()
                .AddGoogle(options =>
                {
                    var config = Configuration.GetSection("Authentication:Google");
                    options.ClientId = config["ClientId"];
                    options.ClientSecret = config["ClientSecret"];
                })
                .AddFacebook(options =>
                {
                    var config = Configuration.GetSection("Authentication:Facebook");
                    options.ClientId = config["ClientId"];
                    options.ClientSecret = config["ClientSecret"];
                })
                .Services
                .ConfigureApplicationCookie(options =>
                {
                    options.Cookie.Name = "auth";
                    options.Cookie.HttpOnly = false;
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.LoginPath = null;
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = redirectContext =>
                        {
                            redirectContext.HttpContext.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                    };
                });
            services
                .AddMvc();

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddRollbarDotNetLogger(app.ApplicationServices);
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var context = serviceScope.ServiceProvider.GetService<MeredithDbContext>())
            {
                context.Database.Migrate();
            }

            app
                .UseAuthentication()
                .UseStaticFiles()
                .UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v0/swagger.json", "Interface API v0"); })
                .UseMvc();
        }
    }
}