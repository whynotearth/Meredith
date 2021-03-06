﻿using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

namespace WhyNotEarth.Meredith.App.DbContext
{
    public static class DbContextExtensions
    {
        public static IApplicationBuilder UseDbContext(this IApplicationBuilder app, IDbContext dbContext)
        {
            dbContext.Database.Migrate();

            return app;
        }
    }
}