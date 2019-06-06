namespace WhyNotEarth.Meredith.Data.Entity
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;

    public class MeredithDbContext : IdentityDbContext<User, Role, int>
    {
        public DbSet<Amenity> Amenities { get; set; }

        public DbSet<Bed> Beds { get; set; }

        public DbSet<Card> Cards { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Page> Pages { get; set; }

        public DbSet<Rule> Rules { get; set; }

        public DbSet<Space> Spaces { get; set; }

        public DbSet<StripeAccount> StripeAccounts { get; set; }

        public DbSet<StripeOAuthRequest> StripeOAuthRequests { get; set; }

        public MeredithDbContext(DbContextOptions<MeredithDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasPostgresExtension("uuid-ossp");
            var configurations = typeof(MeredithDbContext).GetTypeInfo()
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