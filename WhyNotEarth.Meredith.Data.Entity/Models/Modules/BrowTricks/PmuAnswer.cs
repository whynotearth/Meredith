using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks
{
    public class PmuAnswer
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public PmuQuestion Question { get; set; } = null!;

        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;

        public string? Answer { get; set; }
    }

    public class PmuAnswerEntityConfig : IEntityTypeConfiguration<PmuAnswer>
    {
        public void Configure(EntityTypeBuilder<PmuAnswer> builder)
        {
            builder.ToTable("PmuAnswers", "ModuleBrowTricks");
        }
    }
}