using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Emails;
using WhyNotEarth.Meredith.Hotel;
using WhyNotEarth.Meredith.Public;
using WhyNotEarth.Meredith.Shop;
using WhyNotEarth.Meredith.Stripe;
using WhyNotEarth.Meredith.Twilio;
using WhyNotEarth.Meredith.UrlShortener;
using WhyNotEarth.Meredith.Volkswagen;
using Product = WhyNotEarth.Meredith.Public.Product;
using ShoppingProduct = WhyNotEarth.Meredith.Shop.Product;

namespace WhyNotEarth.Meredith
{
    public interface IDbContext : IAsyncDisposable
    {
        DbSet<Card> Cards { get; set; }

        DbSet<Category> Categories { get; set; }

        DbSet<ProductCategory> ProductCategories { get; set; }

        DbSet<Company> Companies { get; set; }

        DbSet<Image> Images { get; set; }

        DbSet<Video> Videos { get; set; }

        DbSet<Page> Pages { get; set; }

        DbSet<SendGridAccount> SendGridAccounts { get; set; }

        DbSet<StripeAccount> StripeAccounts { get; set; }

        DbSet<StripeOAuthRequest> StripeOAuthRequests { get; set; }

        DbSet<Keyword> Keywords { get; set; }

        DbSet<Setting> Settings { get; set; }

        DbSet<Email> Emails { get; set; }

        DbSet<EmailEvent> EmailEvents { get; set; }

        DbSet<ShortMessage> ShortMessages { get; set; }

        DbSet<User> Users { get; set; }

        DbSet<Role> Roles { get; set; }

        DbSet<TwilioAccount> TwilioAccounts { get; set; }

        DbSet<ShortUrl> ShortUrls { get; set; }

        // Shop
        DbSet<Public.Tenant> Tenants { get; set; }

        DbSet<Reservation> Reservations { get; set; }

        DbSet<Price> Prices { get; set; }

        DbSet<Payment> Payments { get; set; }

        DbSet<ShoppingProduct> ShoppingProducts { get; set; }

        DbSet<Variation> Variations { get; set; }

        DbSet<ProductAttribute> ProductAttributes { get; set; }

        DbSet<ProductLocationInventory> ProductLocationInventories { get; set; }

        DbSet<Location> Locations { get; set; }

        DbSet<BusinessHour> BusinessHours { get; set; }

        // Remove in next steps
        DbSet<Product> Products { get; set; }

        // Hotel
        DbSet<Amenity> Amenities { get; set; }

        DbSet<Bed> Beds { get; set; }

        DbSet<Hotel.Hotel> Hotels { get; set; }

        DbSet<Language> Languages { get; set; }

        DbSet<Room> Rooms { get; set; }

        DbSet<RoomType> RoomTypes { get; set; }

        DbSet<Rule> Rules { get; set; }

        DbSet<Space> Spaces { get; set; }

        // Volkswagen
        DbSet<Article> Articles { get; set; }

        DbSet<JumpStart> JumpStarts { get; set; }

        DbSet<Recipient> Recipients { get; set; }

        DbSet<Memo> Memos { get; set; }

        DbSet<NewJumpStart> NewJumpStarts { get; set; }

        // BrowTricks
        DbSet<Client> Clients { get; set; }

        DbSet<FormTemplate> FormTemplates { get; set; }

        DbSet<FormItem> FormItems { get; set; }

        DbSet<FormSignature> FormSignatures { get; set; }

        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}