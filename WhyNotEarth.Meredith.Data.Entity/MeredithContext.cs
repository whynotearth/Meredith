namespace WhyNotEarth.Meredith.Data.Entity
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public class MeredithContext : IdentityDbContext<User, Role, Guid>
    {
        public MeredithContext(DbContextOptions<MeredithContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var configurations = typeof(MeredithContext).GetTypeInfo()
                .Assembly
                .GetTypes()
                .Where(t => t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract
                                                    && t.GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType &&
                                                                                  i.GetGenericTypeDefinition() ==
                                                                                  typeof(IEntityTypeConfiguration<>)))
                .ToList()
                .Select(Activator.CreateInstance)
                .ToList();
            foreach (dynamic configuration in configurations)
            {
                modelBuilder.ApplyConfiguration(configuration);
            }
        }
    }
}