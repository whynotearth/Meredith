namespace WhyNotEarth.Meredith.App
{
    using System;
    using System.Linq;
    using Company;
    using Data.Entity;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Pages;
    using RollbarDotNet.Configuration;
    using RollbarDotNet.Core;
    using RollbarDotNet.Logger;
    using Stripe;
    using Stripe.Data;
    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

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
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin()))
                .AddRollbarWeb()
                .AddOptions()
                .Configure<RollbarOptions>(options => Configuration.GetSection("Rollbar").Bind(options))
                .Configure<StripeOptions>(o => Configuration.GetSection("Stripe").Bind(o))
                .Configure<PageDatabaseOptions>(o => Configuration.GetSection("PageDatabase").Bind(o))
                .AddDbContext<MeredithDbContext>(o => o.UseNpgsql(Configuration.GetConnectionString("Default"),
                    options => options.SetPostgresVersion(new Version(9, 6))))
                .AddScoped<StripeServices>()
                .AddScoped<StripeOAuthServices>()
                .AddSingleton<PageDatabase>()
                .AddScoped<CompanyService>()
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
                })
                .AddMvc();

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddRollbarDotNetLogger(app.ApplicationServices);
            
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var context = serviceScope.ServiceProvider.GetService<MeredithDbContext>())
            {
                context.Database.Migrate();
            }

            app
                .UseStaticFiles()
                .UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v0/swagger.json", "Interface API v0"); })
                .UseMvc();
        }
    }
}