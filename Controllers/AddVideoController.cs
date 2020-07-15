using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bundeswort.Scraper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Bundeswort.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddVideoController : ControllerBase
    {
        private readonly ILogger<AddVideoController> _logger;
        private readonly IDistributedCache distributedCache;

        public AddVideoController(ILogger<AddVideoController> logger, IDistributedCache distributedCache)
        {
            this._logger = logger;
            this.distributedCache = distributedCache;
        }

        [HttpPost]
        public async Task<int> AddVideo([FromBody] VideoDetails videoDetails)
        {
            var cached = await distributedCache.GetAsync(videoDetails.VideoId);
            if(cached == null)
            {
                CaptionsScraper scraper = new CaptionsScraper(new Client());
                var res = await scraper.Scrap(videoDetails.VideoId, new string[] { videoDetails.Language.ToLower() });
                var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1))
                .SetAbsoluteExpiration(TimeSpan.FromDays(6));
                await distributedCache.SetAsync(videoDetails.VideoId, Encoding.UTF8.GetBytes("true"), options);
                return res.Count;
            }
            else
            {
                return 0;
            }
        }
    }

    public class VideoDetails
    {
        public string VideoId { get; set; }
        public string ChannelId { get; set; }
        public string Language { get; set; }
    }
}
