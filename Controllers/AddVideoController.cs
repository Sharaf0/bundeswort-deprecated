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

        private async Task<List<QueuedVideo>> GetRelatedVideos(string videoId, string language)
        {
            try
            {
                List<SearchResult> res = new List<SearchResult>();

                var _youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyBEenDioENoMs9rQbve5gOKN4vJ3c-OuBc",
                    ApplicationName = "crawler"//this.GetType().ToString()
                });

                var searchListRequest = _youtubeService.Search.List("snippet");
                searchListRequest.MaxResults = 10;
                searchListRequest.Type = "video";
                searchListRequest.RelatedToVideoId = videoId;
                searchListRequest.RelevanceLanguage = language;

                var searchListResponse = await searchListRequest.ExecuteAsync();

                res.AddRange(searchListResponse.Items);

                return res
                .Where(s => s.Id != null && !string.IsNullOrEmpty(s.Id.VideoId))
                .Select(s => new QueuedVideo
                {
                    VideoId = s.Id.VideoId,
                    ChannelId = s.Snippet.ChannelId,
                    ChannelTitle = s.Snippet.ChannelTitle,
                    Description = s.Snippet.Description,
                    Etag = s.Snippet.Description,
                    HighThumbnail = s.Snippet.Thumbnails.High.Url,
                    Language = language,
                    PublishedAt = DateTime.Parse(s.Snippet.PublishedAt),
                    VideoTitle = s.Snippet.Title
                })
                .ToList();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        [HttpPost]
        public async Task<int> AddVideo([FromBody] VideoDetails videoDetails, bool clear = false)
        {
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
                .SetSlidingExpiration(Constants.SlidingExpiration)
                .SetAbsoluteExpiration(Constants.AbsoluteExpiration);
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

                //Insert in the queue
                var relatedVideos = await GetRelatedVideos(videoDetails.VideoId, videoDetails.Language);
                foreach (var relatedVideo in relatedVideos)
                {
                    if (context.QueuedVideos.FirstOrDefault(qv => qv.VideoId == relatedVideo.VideoId) != null)
                    {
                        //check etags, if different, update
                    }
                    else
                    {
                        await context.QueuedVideos.AddAsync(relatedVideo);
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
}
