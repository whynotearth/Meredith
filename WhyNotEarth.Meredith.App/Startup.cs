﻿using System;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using RollbarDotNet.Core;
using RollbarDotNet.Logger;
using WhyNotEarth.Meredith.App.Auth;
using WhyNotEarth.Meredith.App.Configuration;
using WhyNotEarth.Meredith.App.DbContext;
using WhyNotEarth.Meredith.App.Localization;
using WhyNotEarth.Meredith.App.Middleware;
using WhyNotEarth.Meredith.App.Swagger;
using WhyNotEarth.Meredith.DependencyInjection;
using WhyNotEarth.Meredith.Persistence;
using WhyNotEarth.Meredith.Volkswagen.Jobs;

[assembly: ApiController]
namespace WhyNotEarth.Meredith.App
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomCorsPolicy(_environment);

            services.AddRollbarWeb();

            services.AddCustomOptions(_configuration);

            services.AddDbContext<MeredithDbContext>(o => o.UseNpgsql(_configuration.GetConnectionString("Default"),
                options => options.SetPostgresVersion(new Version(9, 6))));

            services.AddMeredith();
            services.AddPersistence();

            services.AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User)
                .Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                    options.ForwardLimit = null;
                    options.RequireHeaderSymmetry = false;
                    options.KnownNetworks.Clear();
                    options.KnownProxies.Clear();
                });

            services.AddSwagger(_environment);

            services.AddHangfire(c => c.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(_configuration.GetConnectionString("Default")));

            services.AddHangfireServer();

            services.AddCustomAuthentication(_configuration);

            services.AddCustomAuthorization();

            services.AddControllers()
                .AddMvcOptions(options =>
                {
                    // https://github.com/dotnet/aspnetcore/issues/11584
                    options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider());
                }).AddNewtonsoftJson(c =>
                    c.SerializerSettings.Converters.Add(new StringEnumConverter())
                );
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IRecurringJobManager recurringJobManager,
            IDbContext dbContext)
        {
            app.UseForwardedHeaders();

            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                loggerFactory.AddRollbarDotNetLogger(app.ApplicationServices);
            }

            app.UseDbContext(dbContext);

            app.UseCustomLocalization();

            app.UseCustomSwagger(_environment);

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseCustomAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapHangfireDashboard(new DashboardOptions
                {
                    Authorization = new IDashboardAuthorizationFilter[] { }
                })
                    .RequireAuthorization(Policies.Developer);
            });

            app.UseHangfireDashboard();

            recurringJobManager.AddOrUpdate<JumpStartJob>(JumpStartJob.Id,
                job => job.SendAsync(),
                JumpStartJob.CronExpression, TimeZoneInfo.Utc);

            recurringJobManager.AddOrUpdate<NewJumpStartJob>(NewJumpStartJob.Id,
                job => job.SendAsync(),
                NewJumpStartJob.CronExpression, TimeZoneInfo.Utc);
        }
    }
}