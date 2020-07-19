using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            //TODO: SQL Injection handling
            List<Caption> ftsRes = new List<Caption>(), cntRes = new List<Caption>();
            
            var sql = string.Format(@"SELECT * FROM ""Captions"" WHERE to_tsvector(""Text"") @@ to_tsquery('{0}')", query);
            
            ftsRes = context.Captions.FromSqlRaw(sql).ToList();

            if(ftsRes.Count == 0)
                cntRes = context.Captions.Where(c => c.Text.ToLower().Contains(query.ToLower())).ToList();

            var res = ftsRes.Union(cntRes);

            return res.Select(c => new VideoResult
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
