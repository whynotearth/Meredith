using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;

namespace WhyNotEarth.Meredith.Data.Entity
{
    public class MeredithDbContext : IdentityDbContext<User, Role, int>
    {
        // Public
        public DbSet<Card> Cards { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Page> Pages { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<SendGridAccount> SendGridAccounts { get; set; }

        public DbSet<StripeAccount> StripeAccounts { get; set; }

        public DbSet<StripeOAuthRequest> StripeOAuthRequests { get; set; }

        // Hotel
        public DbSet<Amenity> Amenities { get; set; }

        public DbSet<Bed> Beds { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<Price> Prices { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<RoomType> RoomTypes { get; set; }

        public DbSet<Rule> Rules { get; set; }

        public DbSet<Space> Spaces { get; set; }

        // Volkswagen
        public DbSet<Post> Posts { get; set; }

        public DbSet<JumpStart> JumpStarts { get; set; }

        public DbSet<Recipient> Recipients { get; set; }

        public MeredithDbContext(DbContextOptions<MeredithDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("uuid-ossp");
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeredithDbContext).Assembly);
        }
    }
}