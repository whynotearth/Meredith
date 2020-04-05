using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class User : IdentityUser<int>, IEntityTypeConfiguration<User>
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string GoogleLocation { get; set; }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
        }
    }
}