using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bundeswort.Scraper;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Nest;
using Scraper;
using Video = Scraper.Video;
using Caption = Scraper.Caption;

namespace Bundeswort.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddVideoController : ControllerBase
    {
        private readonly ILogger<AddVideoController> _logger;
        private readonly IDistributedCache distributedCache;
        private readonly VideosDbContext context;
        private readonly ElasticClient esClient;

        public AddVideoController(ILogger<AddVideoController> logger, IDistributedCache distributedCache, VideosDbContext context, ElasticClient esClient)
        {
            this._logger = logger;
            this.distributedCache = distributedCache;
            this.context = context;
            this.esClient = esClient;
        }
        public Task<List<SearchResult>> GetVideosFromChannelAsync(string ytChannelId)
        {

            return Task.Run(() =>
            {
                List<SearchResult> res = new List<SearchResult>();

                var _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyBEenDioENoMs9rQbve5gOKN4vJ3c-OuBc",
                    ApplicationName = "crawler"//this.GetType().ToString()
                });

                string nextpagetoken = " ";

                while (nextpagetoken != null)
                {
                    var searchListRequest = _youtubeService.Search.List("snippet");
                    searchListRequest.MaxResults = 50;
                    searchListRequest.ChannelId = ytChannelId;
                    searchListRequest.PageToken = nextpagetoken;

                    // Call the search.list method to retrieve results matching the specified query term.
                    var searchListResponse = searchListRequest.Execute();

                    // Process  the video responses 
                    res.AddRange(searchListResponse.Items);

                    nextpagetoken = searchListResponse.NextPageToken;

                }

                return res;

            });
        }
        [HttpPost]
        public async Task<int> AddVideo([FromBody] VideoDetails videoDetails, bool clear = false)
        {
            if (videoDetails.ChannelId != null)
            {
                var results = GetVideosFromChannelAsync(videoDetails.ChannelId);
            }
            //Check the cache
            var cached = await distributedCache.GetAsync(videoDetails.VideoId);

            if (cached != null && clear)
            {
                await distributedCache.RemoveAsync(videoDetails.VideoId);
                cached = await distributedCache.GetAsync(videoDetails.VideoId);
            }

            if (cached == null)
            {
                //Check the db
                var video = context.Videos.FirstOrDefault(v => v.VideoId == videoDetails.VideoId);
                if (video != null)
                {
                    if (clear)
                    {
                        context.Videos.Remove(video);
                        context.Captions.RemoveRange(context.Captions.Where(c => c.VideoId == video.VideoId));
                    }
                    else
                        return 0;
                }
                //Insert in the cache
                var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1))
                .SetAbsoluteExpiration(TimeSpan.FromDays(6));
                await distributedCache.SetAsync(videoDetails.VideoId, Encoding.UTF8.GetBytes("true"), options);

                CaptionsScraper scraper = new CaptionsScraper(new Client());
                var res = await scraper.Scrap(videoDetails.VideoId, new string[] { videoDetails.Language.ToLower() });

                foreach (var item in res)
                {
                    await context.Videos.AddAsync(new Video { VideoId = videoDetails.VideoId, Language = videoDetails.Language });
                    foreach (var cp in item.CaptionParts)
                    {
                        var caption =
                            new Caption
                            {
                                VideoId = videoDetails.VideoId,
                                Text = cp.Text,
                                Start = cp.Start,
                                Duration = cp.Duration,
                            };
                        //Insert in DB
                        var q = await context.Captions.AddAsync(caption);
                        await context.SaveChangesAsync();
                        //index the caption
                        var response = await esClient.IndexAsync(caption, idx => idx.Index("caption-index"));
                    }
                }
                return await context.SaveChangesAsync();
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
