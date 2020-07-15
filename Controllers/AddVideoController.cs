using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Bundeswort.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddVideoController : ControllerBase
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

        public AddVideoController(ILogger<SearchController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void AddVideo([FromBody] VideoDetails VideoDetails)
        {
        }
    }

    public class VideoDetails
    {
        public string VideoId { get; set; }
        public string ChannelId { get; set; }
        public string Language { get; set; }
    }
}
