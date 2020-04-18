using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class Memo : IEntityTypeConfiguration<Memo>
    {
        public int Id { get; set; }

        public string Subject { get; set; }
        
        public string Date { get; set; }
        
        public string To { get; set; }
        
        public string Description { get; set; }

        public string DistributionGroup { get; set; }
        
        public void Configure(EntityTypeBuilder<Memo> builder)
        {
            builder.ToTable("Memos", "ModuleVolkswagen");
        }
    }
}