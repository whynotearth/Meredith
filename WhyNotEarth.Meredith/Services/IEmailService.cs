using System.Collections.Generic;
using System.Threading.Tasks;
using WhyNotEarth.Meredith.Hotel;

namespace WhyNotEarth.Meredith.Services
{
    public interface IEmailService
    {
        Task SendReservationEmail(HotelReservation hotelReservation, RoomType roomType, IEnumerable<HotelPrice> dailyPrices,
            decimal vatAmount, int paidDays, string? country, string phoneNumber);
    }
}