using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Scraper;

namespace Bundeswort.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private static readonly string[] Videos = new[]
        {
            "MMV8Sy1wGNg",
            "fIQLH6Qf2-g",
            "z1A1dP0-rxE",
            "i0Uw7-MpfqQ",
            "PfApuJGnpwo",
            "BWVpbudAlVc",
            "4HLqMfy_gMA",
            "hqoSs7nc0Pk",
            "XQX_MyM0Yz4"
        };

        private readonly ILogger<SearchController> _logger;
        private readonly VideosDbContext context;

        public SearchController(ILogger<SearchController> logger, VideosDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        [HttpGet("{query}")]
        public IEnumerable<VideoResult> Get(string query)
        {
            return context.Captions
            .Where(c => c.Text.ToLower().Contains(query.ToLower()))
            .OrderBy(c => c.Start)
            .Select(c => new VideoResult
            {
                VideoId = c.VideoId,
                From = (int)Math.Floor(c.Start),
                To = (int)Math.Floor(c.Start + c.Duration),
                Text = c.Text
            });

            // var rng = new Random();
            // var videos = Videos.Select(v => new VideoResult
            // {
            //     From = rng.Next(10, 100),
            //     VideoId = v
            // }).ToList();

            // videos.ForEach((v) =>
            // {
            //     v.To = v.From + 5;
            // });

            // return videos.ToArray();
        }
    }
}
