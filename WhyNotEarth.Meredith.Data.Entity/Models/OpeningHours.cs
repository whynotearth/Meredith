using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhyNotEarth.Meredith.Data.Entity.Models
{
    public class OpeningHours : IEntityTypeConfiguration<OpeningHours>
    {
        public int Id { get; set; }
        
        public DateTime DayOneOpeningTime { get; set; }

        public DateTime DayOneClosingTime { get; set; }
                
        public DateTime DayTwoOpeningTime { get; set; }

        public DateTime DayTwoClosingTime { get; set; }

        public DateTime DayThreeOpeningTime { get; set; }

        public DateTime DayThreeClosingTime { get; set; }

        public DateTime DayFourOpeningTime { get; set; }

        public DateTime DayFourClosingTime { get; set; }

        public DateTime DayFiveOpeningTime { get; set; }

        public DateTime DayFiveClosingTime { get; set; }

        public DateTime DaySixOpeningTime { get; set; }

        public DateTime DaySixClosingTime { get; set; }

        public DateTime DaySevenOpeningTime { get; set; }

        public DateTime DaySevenClosingTime { get; set; }

        public bool OpenAlways { get; set; }

        public void Configure(EntityTypeBuilder<OpeningHours> builder)
        {
        }
    }
}
