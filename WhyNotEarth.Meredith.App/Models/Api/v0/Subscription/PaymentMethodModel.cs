using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Models.Api.v0.Subscription
{

    public class PaymentMethodModel
    {
        public PaymentCard.Brands Brand { get; set; }

        public string Last4 { get; set; } = null!;

        public int ExpirationMonth { get; set; }

        public int ExpirationYear { get; set; }
    }
}