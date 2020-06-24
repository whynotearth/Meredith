using Slugify;

namespace WhyNotEarth.Meredith.Public
{
    public class SlugService
    {
        public string GetSlug(string name, int entityId)
        {
            return GetSlug($"{name} {entityId}");
        }

        public string GetSlug(string name)
        {
            return new SlugHelper().GenerateSlug(name);
        }
    }
}