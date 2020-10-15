using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Scraper;

namespace Bundeswort.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly VideosDbContext context;
        private readonly IWorker worker;
        private readonly AddVideoController controller;

        public QueueController(VideosDbContext context, IWorker worker, AddVideoController controller)
        {
            this.context = context;
            this.worker = worker;
            this.controller = controller;
        }

        [HttpGet]
        public List<QueuedVideo> Queue()
        {
            return this.context.QueuedVideos.ToList().OrderByDescending(x => x.ViewCount).ToList();
        }

        [HttpPost]
        public async Task<bool> Start()
        {
            try
            {
                var top = this.context.QueuedVideos.ToList().OrderByDescending(x => x.ViewCount).ToList().FirstOrDefault();
                await this.controller.AddVideo(new VideoDetails
                {
                    Language = top.Language,
                    VideoId = top.VideoId
                });
                this.context.QueuedVideos.Remove(top);
                this.context.SaveChanges();
                // var timer = new System.Threading.Timer(e =>
                // {
                //     worker.Run();
                // }, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}