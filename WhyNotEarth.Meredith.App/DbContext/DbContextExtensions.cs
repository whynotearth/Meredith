using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WhyNotEarth.Meredith.Data.Entity;
using WhyNotEarth.Meredith.Data.Entity.Models;

namespace WhyNotEarth.Meredith.App.DbContext
{
    public static class DbContextExtensions
    {
        public static IApplicationBuilder UseDbContext(this IApplicationBuilder app, IWebHostEnvironment env,
            MeredithDbContext dbContext, UserManager<User> userManager)
        {
            dbContext.Database.Migrate();

            if (env.IsDevelopment())
            {
                DbContextSeeder.Seed(dbContext, userManager);
            }

            return app;
        }
    }
}