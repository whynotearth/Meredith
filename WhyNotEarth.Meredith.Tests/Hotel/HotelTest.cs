namespace WhyNotEarth.Meredith.Hotel
{
    using Microsoft.Extensions.DependencyInjection;
    using WhyNotEarth.Meredith.Tests.Data;

    public class HotelTest : DatabaseContextTest
    {
        protected HotelUtility HotelUtility { get; }

        public HotelTest()
        {
            HotelUtility = ServiceProvider.GetRequiredService<HotelUtility>();
        }
    }
}