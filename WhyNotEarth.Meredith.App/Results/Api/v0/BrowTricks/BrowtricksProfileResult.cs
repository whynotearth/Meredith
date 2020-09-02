using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.App.Results.Api.v0.Public;
using WhyNotEarth.Meredith.BrowTricks;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class BrowtricksProfileResult
    {
        public string Name { get; }

        public List<ImageResult>? Images { get; }

        public List<VideoResult>? Videos { get; }

        public BrowtricksProfileResult(User user, List<ClientImage> images, List<ClientVideo> videos)
        {
            Name = user.FullName;
            Images = images.Select(item => new ImageResult(item)).ToList();
            Videos = videos.Select(item => new VideoResult(item)).ToList();
        }
    }
}