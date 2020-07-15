using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bundeswort.Scraper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bundeswort.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddVideoController : ControllerBase
    {
        private readonly ILogger<AddVideoController> _logger;

        public AddVideoController(ILogger<AddVideoController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<int> AddVideo([FromBody] VideoDetails videoDetails)
        {
            CaptionsScraper scraper = new CaptionsScraper(new Client());
            var res = await scraper.Scrap(videoDetails.VideoId, new string[] { videoDetails.Language.ToLower() });
            return res.Count;
        }
    }

    public class VideoDetails
    {
        public string VideoId { get; set; }
        public string ChannelId { get; set; }
        public string Language { get; set; }
    }
}
