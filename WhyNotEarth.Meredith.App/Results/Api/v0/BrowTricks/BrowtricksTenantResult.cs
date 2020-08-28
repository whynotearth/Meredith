using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class BrowtricksTenantResult
    {
        public List<ImageResult>? Images { get; }

        public List<VideoResult>? Videos { get; }

        public BrowtricksTenantResult(List<ClientImage> images, List<ClientVideo> videos)
        {
            Images = images.Select(item => new ImageResult(item)).ToList();
            Videos = videos.Select(item => new VideoResult(item)).ToList();
        }
    }
}