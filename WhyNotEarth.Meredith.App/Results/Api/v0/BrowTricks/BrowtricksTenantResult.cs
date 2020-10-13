using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public.Tenant;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class BrowtricksTenantResult
    {
        public TenantResult Tenant { get; }

        public List<ImageResult>? Images { get; }

        public List<VideoResult>? Videos { get; }

        public BrowtricksTenantResult(Meredith.Public.Tenant tenant, List<BrowTricksImage> images, List<BrowTricksVideo> videos)
        {
            Tenant = new TenantResult(tenant);
            Images = images.Select(item => new ImageResult(item)).ToList();
            Videos = videos.Select(item => new VideoResult(item)).ToList();
        }
    }
}