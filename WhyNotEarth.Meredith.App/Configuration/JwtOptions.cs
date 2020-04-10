namespace WhyNotEarth.Meredith.App.Configuration
{
    public class JwtOptions
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public string Key { get; set; }
        
        public string Issuer { get; set; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

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