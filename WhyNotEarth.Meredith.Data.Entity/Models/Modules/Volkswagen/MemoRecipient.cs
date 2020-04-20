using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WhyNotEarth.Meredith.Data.Entity.Models.Modules.Volkswagen
{
    public class MemoRecipient : IEntityTypeConfiguration<MemoRecipient>
    {
        public int Id { get; set; }

        public int MemoId { get; set; }

        public Memo Memo { get; set; }

        public string Email { get; set; }

        public string DistributionGroup { get; set; }

        public MemoStatus Status { get; set; }

        public void Configure(EntityTypeBuilder<MemoRecipient> builder)
        {
            builder.ToTable("MemoRecipients", "ModuleVolkswagen");
        }
    }

    public enum MemoStatus : byte
    {
        None = 0,
        ReadyToSend = 1,
        Sent = 2,
        Delivered = 3,
        Opened = 4,
        Clicked = 5
    }
}