namespace WhyNotEarth.Meredith.Identity
{
    public class JwtOptions
    {
        public string Key { get; set; } = null!;

        public string Issuer { get; set; } = null!;

        public int ExpireDays { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(Issuer) || ExpireDays == 0)
            {
                return false;
            }

            return true;
        }
    }
}