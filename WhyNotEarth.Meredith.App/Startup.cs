namespace WhyNotEarth.Meredith.App
{
    using Company;
    using Data.Entity;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Pages;

    public class Startup
    {
        protected IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddOptions()
                .Configure<PageDatabaseOptions>(o => Configuration.GetSection("PageDatabase").Bind(o))
                .AddDbContext<MeredithContext>(o => o.UseNpgsql(Configuration.GetConnectionString("Default")))
                .AddSingleton<PageDatabase>()
                .AddScoped<CompanyService>()
                .AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            using (var context = serviceScope.ServiceProvider.GetService<MeredithContext>())
            {
                context.Database.EnsureCreated();
            }
            
            app
                .UseStaticFiles()
                .UseMvc();
        }
    }
}