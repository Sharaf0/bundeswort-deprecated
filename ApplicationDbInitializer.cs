using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scraper;
using System.Linq;
using Bundeswort.Controllers;

namespace Bundeswort
{
    public class ApplicationDbInitializer
    {
        public static void SeedVideos(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<VideosDbContext>();
                var controller = scope.ServiceProvider.GetRequiredService<AddVideoController>();
                if (context.Videos.Count() == 0)
                {
                    string[] videos = { "OxfUqR1CHNA", "mTXXrqN4aYI", "kO4-RYvb9Uw", "seDN7LlXQZo", "7z8sUwtpUvY", "_slAaVmJerc", "2myOjbDmtKY", "WaDQyffikqA", "ojToYs6nCnk" };
                    foreach (var v in videos)
                    {
                        var res = controller.AddVideo(new VideoDetails{VideoId = v, Language = "DE"}).Result;
                    }
                }
            }
        }
    }
}

