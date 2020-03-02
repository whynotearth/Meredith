using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RollbarDotNet.Configuration;
using RollbarDotNet.Core;
using RollbarDotNet.Logger;
using WhyNotEarth.Meredith.App.Configuration;
using WhyNotEarth.Meredith.App.ConfigureServices;
using WhyNotEarth.Meredith.App.Localization;
using WhyNotEarth.Meredith.App.Middleware;
using WhyNotEarth.Meredith.Cloudinary;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.DependencyInjection;
using WhyNotEarth.Meredith.Stripe.Data;

namespace WhyNotEarth.Meredith.App
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o
                .AddDefaultPolicy(builder => builder
                    .SetIsOriginAllowed(origin => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()));

            services.AddRollbarWeb();

            services.AddOptions()
                .Configure<CloudinaryOptions>(o => _configuration.GetSection("Cloudinary").Bind(o))
                .Configure<RollbarOptions>(o => _configuration.GetSection("Rollbar").Bind(o))
                .Configure<SendGridOptions>(options => _configuration.GetSection("SendGrid").Bind(options))
                .Configure<StripeOptions>(o => _configuration.GetSection("Stripe").Bind(o))
                .Configure<JwtOptions>(o => _configuration.GetSection("Jwt").Bind(o));

            services.AddDbContext<MeredithDbContext>(o => o.UseNpgsql(_configuration.GetConnectionString("Default"),
                options => options.SetPostgresVersion(new Version(9, 6))));

            services.AddMeredith();

            services.AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User)
                .Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    options.ForwardLimit = null;
                    options.RequireHeaderSymmetry = false;
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                });

            services.AddSwagger();

            services.AddCustomAuthentication(_configuration);

            services.AddControllers()
                .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
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
                .UseCustomLocalization()
                .UseAuthentication()
                .Use(async (context, next) =>
                {
                    // If the default identity failed to authenticate (cookies)
                    if (context.User.Identities.All(i => !i.IsAuthenticated))
                    {
                        var principal = new ClaimsPrincipal();
                        var jwtAuth = await context.AuthenticateAsync("jwt");
                        if (jwtAuth?.Principal != null)
                        {
                            principal.AddIdentities(jwtAuth.Principal.Identities);
                            context.User = principal;
                        }
                    }

                    await next();
                })
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v0/swagger.json", "Interface API v0");
                    c.RoutePrefix = string.Empty;
                })
                .UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("default");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}