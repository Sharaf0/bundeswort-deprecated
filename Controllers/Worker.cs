using System.Linq;
using Bundeswort.Controllers;
using Microsoft.Extensions.Logging;
using Scraper;

public interface IWorker
{
    void Run();
}

public class Worker : IWorker
{
    private readonly ILogger<Worker> _logger;
    private readonly VideosDbContext context;
    private readonly AddVideoController videoController;
    public Worker(ILogger<Worker> logger, VideosDbContext context, AddVideoController videoController)
    {
        this._logger = logger;
        this.context = context;
        this.videoController = videoController;
    }
    public async void Run()
    {
        var top = this.context.QueuedVideos
        .OrderByDescending(x => x.ViewCount)
        .ThenByDescending(x => x.LikeCount)
        .ThenBy(x => x.DislikeCount)
        .FirstOrDefault();

        await this.videoController.AddVideo(new VideoDetails
        {
            Language = top.Language,
            VideoId = top.VideoId
        });
    }
}