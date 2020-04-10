using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Data.Entity.Models.Modules.Hotel;

namespace WhyNotEarth.Meredith.Services
{
    public interface IEmailService
    {
        Task SendReservationEmail(Reservation reservation, RoomType roomType, IEnumerable<Price> dailyPrices,
            decimal vatAmount, int paidDays, string? country, string phoneNumber);
    }
}