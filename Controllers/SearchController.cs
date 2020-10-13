using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Scraper;
using NpgsqlTypes;

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
        public struct Res
        {
            public Caption caption { get; set; }
            public NpgsqlTsVector vector { get; set; }
        }

        //TODO: Receive the language too.
        [HttpGet("{query}")]
        public IEnumerable<VideoResult> Get(string query)
        {
            const string lang = "english";
            
            var results = context.Captions
            .Select(c => new Res { caption = c, vector = EF.Functions.ToTsVector(lang, c.Text) })
            .OrderBy(c => c.vector.Rank(EF.Functions.ToTsQuery(lang, query)))
            .Where(c => c.vector.Matches(EF.Functions.ToTsQuery(lang, query)))
            //.Where(c => c.caption.Language.equals(lang))
            .Select(c => c.caption);

            return results.Select(c => new VideoResult
            {
                VideoId = c.VideoId,
                From = (int)Math.Floor(c.Start),
                To = (int)Math.Floor(c.Start + c.Duration),
                Text = c.Text
            });
        }
    }
}
