namespace WhyNotEarth.Meredith.App.Configuration
{
    public class JwtOptions
    {
        public string Key { get; set; }

        public string Issuer { get; set; }

        public int ExpireDays { get; set; }
    }
}