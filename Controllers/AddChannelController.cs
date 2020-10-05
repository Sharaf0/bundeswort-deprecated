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
    public class AddChannelController : ControllerBase
    {
        private readonly ILogger<AddVideoController> _logger;
        private readonly IDistributedCache distributedCache;
        private readonly VideosDbContext context;
        private readonly AddVideoController videoController;

        public AddChannelController(ILogger<AddVideoController> logger, IDistributedCache distributedCache, VideosDbContext context, AddVideoController videoController)
        {
            this._logger = logger;
            this.distributedCache = distributedCache;
            this.context = context;
            this.videoController = videoController;
        }

        private async Task<List<SearchResult>> GetVideosFromChannelAsync(string ytChannelId)
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
                var searchListResponse = await searchListRequest.ExecuteAsync();

                // Process  the video responses 
                res.AddRange(searchListResponse.Items);

                nextpagetoken = searchListResponse.NextPageToken;

            }

            return res;
        }

        [HttpPost]
        public async Task<int> AddVideo([FromBody] ChannelDetails channelDetails)
        {
            if (string.IsNullOrEmpty(channelDetails.ChannelId))
            {
                throw new ArgumentException("Channel Id should not be null or empty");
            }

            //Check the cache
            var cached = await distributedCache.GetAsync(channelDetails.ChannelId);

            if (cached != null)
                return 0;

            //Check the db
            var channel = context.Channels.FirstOrDefault(v => v.ChannelId == channelDetails.ChannelId);
            if (channel != null)
                return 0;

            var channelVideos = await GetVideosFromChannelAsync(channelDetails.ChannelId);
            var videosIds = channelVideos.Select(cv => cv.Id.VideoId).Where(v => !string.IsNullOrEmpty(v)).ToList();

            var counter = 1;
            foreach (var video in videosIds)
            {
                Console.WriteLine($"Done - Video {counter++} / {videosIds.Count}");
                await videoController.AddVideo(new VideoDetails { VideoId = video, Language = channelDetails.Language });
            }

            //Insert in db
            await context.Channels.AddAsync(new YoutubeChannel { ChannelId = channelDetails.ChannelId, Language = channelDetails.Language });

            //Insert in the cache
            var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(Constants.SlidingExpiration)
            .SetAbsoluteExpiration(Constants.AbsoluteExpiration);
            await distributedCache.SetAsync(channelDetails.ChannelId, Encoding.UTF8.GetBytes("true"), options);

            return await context.SaveChangesAsync();
        }
    }
}
