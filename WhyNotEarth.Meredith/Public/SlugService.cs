namespace WhyNotEarth.Meredith.Public
{
    public class SlugService
    {
        public string GetSlug(string name)
        {
            return name.ToLower();
        }
    }
}