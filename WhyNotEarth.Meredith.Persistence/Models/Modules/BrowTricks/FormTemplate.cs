using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.Persistence.Models.Modules.BrowTricks
{
    public class FormTemplateEntityConfig : IEntityTypeConfiguration<FormTemplate>
    {
        public void Configure(EntityTypeBuilder<FormTemplate> builder)
        {
            builder.ToTable("FormTemplates", "ModuleBrowTricks");
        }
    }
}