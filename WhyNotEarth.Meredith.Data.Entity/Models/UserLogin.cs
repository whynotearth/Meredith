namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class UserLogin : IdentityUserLogin<int>, IEntityTypeConfiguration<UserLogin>
    {
        public Site Site { get; set; }

        public int? SiteId { get; set; }

        public void Configure(EntityTypeBuilder<UserLogin> builder)
        {
            builder.ToTable("UserLogins");
        }
    }
}