namespace WhyNotEarth.Meredith.App.Models.Api.v0.Subscription
{
    public class CreateModel
    {
        public int PlanId { get; set; }

        public string? CouponCode { get; set; }
    }
}