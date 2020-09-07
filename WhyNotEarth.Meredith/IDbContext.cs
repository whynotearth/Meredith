using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Identity;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Shop;
using WhyNotEarth.Meredith.Volkswagen;
using Product = WhyNotEarth.Meredith.Public.Product;
using ShoppingProduct = WhyNotEarth.Meredith.Shop.Product;

namespace WhyNotEarth.Meredith
{
    public interface IDbContext : IAsyncDisposable
    {
        public DbSet<Card> Cards { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Video> Videos { get; set; }

        public DbSet<Page> Pages { get; set; }

        public DbSet<SendGridAccount> SendGridAccounts { get; set; }

        public DbSet<StripeAccount> StripeAccounts { get; set; }

        public DbSet<StripeOAuthRequest> StripeOAuthRequests { get; set; }

        public DbSet<Keyword> Keywords { get; set; }

        public DbSet<Setting> Settings { get; set; }

        public DbSet<Public.Email> Emails { get; set; }

        public DbSet<EmailEvent> EmailEvents { get; set; }

        public DbSet<ShortMessage> ShortMessages { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<LoginToken> LoginTokens { get; set; }

        // Shop
        public DbSet<Public.Tenant> Tenants { get; set; }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Price> Prices { get; set; }

        public DbSet<Payment> Payments { get; set; }

        public DbSet<ShoppingProduct> ShoppingProducts { get; set; }

        public DbSet<Variation> Variations { get; set; }

        public DbSet<ProductAttribute> ProductAttributes { get; set; }

        public DbSet<ProductLocationInventory> ProductLocationInventories { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<BusinessHour> BusinessHours { get; set; }

        // Remove in next steps
        public DbSet<Product> Products { get; set; }

        // Hotel
        public DbSet<Amenity> Amenities { get; set; }

        public DbSet<Bed> Beds { get; set; }

        public DbSet<Hotel.Hotel> Hotels { get; set; }

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

        public DbSet<NewJumpStart> NewJumpStarts { get; set; }

        // BrowTricks
        public DbSet<Client> Clients { get; set; }

        public DbSet<FormTemplate> FormTemplates { get; set; }

        public DbSet<FormSignature> FormSignatures { get; set; }

        public DatabaseFacade Database { get; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}