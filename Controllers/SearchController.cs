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
            //SELECT * FROM "Captions" WHERE to_tsvector('german', "Text") @@ to_tsquery('german', 'gekommen');
        }
    }
}
