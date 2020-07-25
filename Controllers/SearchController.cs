using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nest;
using Scraper;

namespace Bundeswort.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly VideosDbContext context;
        private readonly ElasticClient esClient;

        public SearchController(ILogger<SearchController> logger, VideosDbContext context, ElasticClient esClient)
        {
            _logger = logger;
            this.context = context;
            this.esClient = esClient;
        }

        [HttpGet("{query}")]
        public IEnumerable<VideoResult> Get(string query)
        {
            // var res = esClient.Search<Caption>(search => search
            // .Query(q => q.Bool(b => b
            //     .Should(
            //         s => s.Match(m => m.Query(query).Field(f => f.Text).Boost(1.1)),
            //         s => s.Match(m => m.Query(query).Field(f => f.Text).Fuzziness(Fuzziness.EditDistance(2)))
            //     )
            // ))
            // ).Documents.ToList();

            var captions = esClient.Search<Caption>(s => s
                        .Size(10)
                        .Query(q => q.QueryString(
                            qs => qs.Query(query)
                            .AllowLeadingWildcard(true)
                            )
                        )
                    ).Documents.ToList();

            var response = esClient.Search<Caption>(s => s
                .Index("caption-index") //or specify index via settings.DefaultIndex("mytweetindex");
                .From(0)
                .Size(10)
                .Query(q => q
                    .Term(t => t.Text, query) || q
                    .Match(mq => mq.Field(f => f.Text).Query(query))
                )
            );
            return captions.Select(c => new VideoResult
            {
                VideoId = c.VideoId,
                From = (int)Math.Floor(c.Start),
                To = (int)Math.Floor(c.Start + c.Duration),
                Text = c.Text
            });

            // //TODO: SQL Injection handling
            // List<Caption> ftsRes = new List<Caption>(), cntRes = new List<Caption>();

            // var sql = string.Format(@"SELECT * FROM ""Captions"" WHERE to_tsvector(""Text"") @@ to_tsquery('{0}')", query);

            // ftsRes = context.Captions.FromSqlRaw(sql).ToList();

            // if (ftsRes.Count == 0)
            //     cntRes = context.Captions.Where(c => c.Text.ToLower().Contains(query.ToLower())).ToList();

            // var res = ftsRes.Union(cntRes);

            // return res.Select(c => new VideoResult
            // {
            //     VideoId = c.VideoId,
            //     From = (int)Math.Floor(c.Start),
            //     To = (int)Math.Floor(c.Start + c.Duration),
            //     Text = c.Text
            // });
            // //SELECT * FROM "Captions" WHERE to_tsvector('german', "Text") @@ to_tsquery('german', 'gekommen');
        }
    }
}
