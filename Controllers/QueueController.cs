using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Scraper;

namespace Bundeswort.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly VideosDbContext context;

        public QueueController(VideosDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public List<QueuedVideo> Queue()
        {
            return this.context.QueuedVideos.ToList();
        }
    }
}