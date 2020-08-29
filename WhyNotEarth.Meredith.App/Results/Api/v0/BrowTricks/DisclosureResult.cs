using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class DisclosureResult
    {
        public int Id { get; }

        public string Value { get; }

        public DisclosureResult(Disclosure disclosure)
        {
            Id = disclosure.Id;
            Value = disclosure.Value;
        }
    }
}