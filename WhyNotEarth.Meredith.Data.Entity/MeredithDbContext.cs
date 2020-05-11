using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WhyNotEarth.Meredith.Data.Entity.Models;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Shop;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen;
using Product = WhyNotEarth.Meredith.Data.Entity.Models.Product;

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

        public DbSet<SendGridAccount> SendGridAccounts { get; set; }

        public DbSet<StripeAccount> StripeAccounts { get; set; }

        public DbSet<StripeOAuthRequest> StripeOAuthRequests { get; set; }

        public DbSet<Keyword> Keywords { get; set; }

        public DbSet<Setting> Settings { get; set; }

        // Shop
        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<HotelPrice> Prices { get; set; }

        public DbSet<Payment> Payments { get; set; }
        
        // Remove in next steps
        public DbSet<Product> Products { get; set; }

        // Hotel
        public DbSet<Amenity> Amenities { get; set; }

        public DbSet<Bed> Beds { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<RoomType> RoomTypes { get; set; }

        public DbSet<Rule> Rules { get; set; }

        public DbSet<Space> Spaces { get; set; }

        // Volkswagen
        public DbSet<Article> Articles { get; set; }

        public DbSet<JumpStart> JumpStarts { get; set; }

        public DbSet<Recipient> Recipients { get; set; }

        public DbSet<Memo> Memos { get; set; }

        public DbSet<EmailRecipient> EmailRecipients { get; set; }

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