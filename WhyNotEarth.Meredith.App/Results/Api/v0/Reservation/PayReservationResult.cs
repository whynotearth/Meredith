namespace WhyNotEarth.Meredith.App.Results.Api.v0.Reservation
{
    public class PayReservationResult
    {
        public string ClientSecret { get; }

        public PayReservationResult(string clientSecret)
        {
            ClientSecret = clientSecret;
        }
    }
}