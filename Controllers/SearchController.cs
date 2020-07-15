using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        public SearchController(ILogger<SearchController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{query}")]
        public IEnumerable<VideoResult> Get(string query)
        {
            var rng = new Random();
            var videos = Videos.Select(v => new VideoResult
            {
                From = rng.Next(10, 100),
                VideoId = v
            }).ToList();

            videos.ForEach((v) =>
            {
                v.To = v.From + 5;
            });

            return videos.ToArray();
        }
    }
}
