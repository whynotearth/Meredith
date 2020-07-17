using System.Collections.Generic;
using System.Linq;

namespace WhyNotEarth.Meredith
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> items,
            int maxItems)
        {
            return items.Select((item, inx) => new { item, inx })
                .GroupBy(x => x.inx / maxItems)
                .Select(g => g.Select(x => x.item).ToList());
        }
    }
}